using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using System;

[StructLayout(LayoutKind.Sequential)]
public struct c_flat_string_vec {
    public IntPtr chars_ptr;
    public UInt64 chars_len;
    public UInt64 chars_cap;
    public IntPtr lens_ptr;
    public UInt64 lens_len;
    public UInt64 lens_cap;
}

#if UNITY_STANDALONE_WIN
public class ControlpadsLibrary {
    [DllImport("c_sharp_controlpads.dll")]
    public static extern int free_c_flat_string_vec(c_flat_string_vec c_flat);
    [DllImport("c_sharp_controlpads.dll")]
    public static extern int clients_changed(ref bool did_change);
    [DllImport("c_sharp_controlpads.dll")]
    public static extern int get_client_handles(ref c_flat_string_vec client_handles);
    [DllImport("c_sharp_controlpads.dll")]
    public static extern int send_message([MarshalAs(UnmanagedType.LPStr)] string client, [MarshalAs(UnmanagedType.LPStr)] string msg);
    [DllImport("c_sharp_controlpads.dll")]
    public static extern int get_messages([MarshalAs(UnmanagedType.LPStr)] string client, ref c_flat_string_vec messages);
}
#elif UNITY_STANDALONE_LINUX
public class ControlpadsLibrary {
    [DllImport("libc_sharp_controlpads.so")]
    public static extern int free_c_flat_string_vec(c_flat_string_vec c_flat);
    [DllImport("libc_sharp_controlpads.so")]
    public static extern int clients_changed(ref bool did_change);
    [DllImport("libc_sharp_controlpads.so")]
    public static extern int get_client_handles(ref c_flat_string_vec client_handles);
    [DllImport("libc_sharp_controlpads.so")]
    public static extern int send_message([MarshalAs(UnmanagedType.LPStr)] string client, [MarshalAs(UnmanagedType.LPStr)] string msg);
    [DllImport("libc_sharp_controlpads.so")]
    public static extern int get_messages([MarshalAs(UnmanagedType.LPStr)] string client, ref c_flat_string_vec messages);
}
#endif


    
public class controlpads_glue : MonoBehaviour
{
    
    [SerializeField]
    private UnityEvent<string, string> _onControlpadMessage;
    
    static List<string> clientHandles = new List<string>();
    
    // Start is called before the first frame update
    void Start()
    {
        clientHandles = GetClientHandles();
        // Debug.Log(string.Join(", ", clientHandles));
    }

    // Update is called once per frame
    void Update()
    {
        if (ClientsChanged()) {
            clientHandles = GetClientHandles();
            // Debug.Log(string.Format("Clients: [{0}]", string.Join(", ", clientHandles)));
        }
        foreach (string client in clientHandles) {
            foreach (string msg in GetMessages(client)) {
                // Debug.Log(string.Format("From <{0}> got <{1}>", client, msg));
                _onControlpadMessage.Invoke(client, msg);
            }
        }
    }

    // Call this function in your own code to send messages to controlpads
    public static void SendControlpadMessage(string client, string message) {
        if (client == null || client.Length == 0 || client.Contains(" ")) {
            // Debug.Log(string.Format("Warning: Invalid client specified for controlpad message <{0}>", message));
            return;
        }
	if (message == null) {
            // Debug.Log("Warning: Tried to send a null controlpad message");
            return;
        }
        int result = ControlpadsLibrary.send_message(client, message);
        if (result != 0)
        {
            // Debug.Log(string.Format("Could send <{1}> to <{0}>: <{2}>", client, message, result));
        }
    }

    
    // ---- Library Glue Functions ----
    bool ClientsChanged() {
        bool b = false;
        int result = ControlpadsLibrary.clients_changed(ref b);
        if (result != 0) {
            // Debug.Log(string.Format("Controlpads Error (clients_changed): {0}", result));
        }
        return b;
    }
    
    List<string> GetClientHandles() {
        c_flat_string_vec c_flat_handles = new c_flat_string_vec();
        int result = ControlpadsLibrary.get_client_handles(ref c_flat_handles);
        if (result != 0) {
            // Debug.Log(string.Format("Controlpads Error (get_client_handles): {0}", result));
        }
        List<string> handles = CFlatToList(c_flat_handles);
        ControlpadsLibrary.free_c_flat_string_vec(c_flat_handles);
        return handles;
    }

    
    List<string> GetMessages(string client) {
        c_flat_string_vec c_flat_messages = new c_flat_string_vec();
        int result = ControlpadsLibrary.get_messages(client, ref c_flat_messages);
        if (result != 0) {
            // Debug.Log(string.Format("Controlpads Error (get_messages): {0}", result));
        }
        List<string> messages = CFlatToList(c_flat_messages);
        ControlpadsLibrary.free_c_flat_string_vec(c_flat_messages);
        return messages;
    }

    // ---- Library Glue Helper ----    
    List<string> CFlatToList(c_flat_string_vec cFlat) {
        List<string> list = new List<string>();
        int char_index = 0;
        for (UInt64 lens_i = 0; lens_i < cFlat.lens_len; lens_i++) {
            unsafe {
                UInt64* u64ptr = (UInt64*)cFlat.lens_ptr;
                u64ptr += lens_i;
                UInt64 len = *u64ptr;
                string s = Marshal.PtrToStringAnsi(cFlat.chars_ptr + char_index, (int)len);
                char_index += (int)len;
                list.Add(s);
            }
        }
        return list;
    }


}

