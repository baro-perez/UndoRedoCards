Test assigment for a recruitment process
For candidate assesment only. See LICENSE file for more info

## What you built
GameManager instantiates the stacks from data from a ScriptableObject
Each card has a suit and number with different sprites
Cards can be drag and dropped between stacks
Card movements are recorded and can be undone/redone

## What you'd improve with more time
Apart from building the solitaire rules, card drag and drop could use better animations, particles, sound, etc
Also drag and drop could take into account the whole area of the card and not just the mouse position

## Which parts were AI-assisted, and how you prompted/used it
For speed and not having to deal with remembering the Unity intricacies myself, ChatGPT was used for the drag and drop code with this prompt:

> I have several unity Image which I want to drag and drop onto eachother (game is a solitaire and I'm moving cards). Please provide drag and drop code. I want the dragged card to follow the mouse while dragging

I already knew the kind of code it would generate and I just tweaked it for my own necessities
