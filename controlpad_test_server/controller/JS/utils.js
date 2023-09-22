// Name Generator. Will pick a random adjective and a random name

const adjList = [
    'Zany', 'Whimsical', 'Bubbly', 'Playful', 'Lively', 
    'Quirky', 'Cheeky', 'Vibrant', 'Jovial', 'Energetic', 
    'Spirited', 'Carefree', 'Groovy', 'Eclectic', 
    'Spontaneous', 'Lighthearted', 'Hilarious', 'Dynamic', 
    'Frolicsome', 'Witty', 'Radiant', 'Wacky', 'Festive', 
    'Silly', 'Mischievous', 'Animated', 'Exuberant', 'Sprightly',
    'Unpredictable', 'Merry', 'Buoyant', 'Fanciful', 
    'Unconventional', 'Chucklesome', 'Bouncy', 'Joyful', 
    'Effervescent', 'Amusing', 'Breezy', 'Giggly', 
    'Frisky', 'Spunky', 'Jaunty', 'Droll', 'Zesty', 
    'Dynamic', 'Peculiar', 'Euphoric', 'Zippy', 'Hysterical'
    ];
const nameList = [
    'Dog', 'Cat', 'Elephant', 'Lion', 'Tiger', 'Giraffe', 
    'Zebra', 'Bear', 'Monkey', 'Kangaroo', 'Dolphin', 
    'Penguin', 'Owl', 'Koala', 'Cheetah', 'Rhino', 'Hippo',
    'Fox', 'Wolf', 'Horse', 'Rabbit', 'Squirrel', 'Gorilla',
    'Orangutan', 'Octopus', 'Crocodile', 'Alligator', 
    'Camel', 'Peacock', 'Parrot', 'Eagle', 'Hummingbird',
    'Sloth', 'Panda', 'Otter', 'Seal', 'Lemur', 'Raccoon',
    'Jellyfish', 'Seahorse', 'Butterfly', 'Meerkat', 
    'Chimpanzee', 'Platypus', 'Hedgehog', 'Llama', 
    'Armadillo', 'Ostrich', 'Tortoise', 'Gazelle'
]


function generateName() {
    return adjList[Math.floor( Math.random() * adjList.length )] + " " + nameList[Math.floor( Math.random() * nameList.length )];
 };