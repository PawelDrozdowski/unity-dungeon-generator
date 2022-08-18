# C# Unity dungeon / hex map generator

Efficient generating up to 30 000 elements

It takes a while for bigger structures

Tested on Unity 2020.1.14f1

[Overview on YouTube](https://youtube.com/playlist?list=PLzskWQnp3wmY3YXI3lnMNNyVnkaUqLDzi)

# Features

1. Chunks
2. Pathfinding
3. Object Pooling
4. Two types of shapes
5. 2x2 tiles (for squares)
6. Seed based generation
7. Three patterns of generation
8. "Ultra speed" toggle for generation

# Examples

Example (small map)

Grey tiles with red center make shortest path betweeen 2 elements

![ScreenShot](https://user-images.githubusercontent.com/93579864/185427669-1d596ab1-1a03-4ba6-ab16-c62620a558c6.png)

Example (big map)

![ScreenShot](https://user-images.githubusercontent.com/93579864/185428073-4f6c562a-2eb1-4550-b39e-25ed4fbb6d18.png)

Room 2x2

![ScreenShot](https://user-images.githubusercontent.com/93579864/185429513-a9cc346f-5ede-4b77-b8c8-0f14fb5ded9d.png)

Singular path generation (Set density to 0)

![gif](https://user-images.githubusercontent.com/93579864/185432712-637877a9-337d-4d48-815c-f2d33e198ba8.gif)

Multi path generation (set density to 2)

![gif](https://user-images.githubusercontent.com/93579864/185431838-9c4e00fe-3fa1-4fcb-bdb6-8ea17aa5c8c8.gif)

Inspector view

![ScreenShot](https://user-images.githubusercontent.com/93579864/185426530-5cb1c468-13da-4280-9315-5cf9dd1adaf1.png)

Tests

![ScreenShot](https://user-images.githubusercontent.com/93579864/185433243-c65e1a6d-6e49-44e9-bb5f-13ed454b1912.png)
