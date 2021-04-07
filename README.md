# Bricked Out

## About <a name = "about"></a>

This is an example Unity Game developed mobile-first (Vulkan/OpenGLES3) showcasing various common Unity patterns and gameplay programming, as well as use of an Event Aggregator pattern to decouple state using many-to-many Pub/Sub. Completed over the course of a couple days.

![gif of the game](https://i.imgur.com/WutrMjg.gif)

## Notes -

I can't upload the translucent image blur shader as it is a paid asset.
Replace these with Image components to build yourself.

There is plenty of room for proper optimizations such as switching to URP and using proper instanced shaders. Batching statically works to a degree but can be far better.

Can bake textures and convert entirely to Unlit shaders. There are no shadows and only one static directional light.

Brick prefabs as well as the fractured prefabs should be converted to use object pooling.

Runs at ~90 fps on Oneplus 7 Pro.


