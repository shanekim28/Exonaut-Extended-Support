# Exonaut Extended Support

## What is this repository?

This git repository contains the decompiled source code from Cartoon Network's **Project Exonaut**. The Unity project you see before you is an attempt to restore Project Exonaut to its working state after it was discontinued in 2014. 

As of September 4, 2020, this Unity project is *NOT* functional. All compiler errors have been fixed, but a lot of reverse engineering still needs to get done before the game works in the editor. Scripts are missing from every GameObject and servers do not exist.

## Is the game playable?

Yes and no. Currently, only the tutorial and hangar are playable. Join the [Discord](https://discord.com/invite/7ca7Qa) for Exonaut Extended Support to find out more.

## When will the game be ready?

I have no idea. As it stands, I am currently the only one working to reverse engineer the game. My initial estimate is somewhere around May 2021. The game itself is still pretty intact, but the server logic has to be completely reversed and rebuilt, along with materials and shaders.

## Can I help?

Yes, yes, a thousand times, yes! Open an issue and I'll tell you directly how you can contribute.

## Technical info for nerds

The original Project Exonaut game was made in Unity3D. It's unclear exactly which version, but I'm guessing it's version 3.5, but to get the project working in Unity, I had to upgrade it to version 5.6. The game is originally written using .NET Framework 2.0 (C# 4.0), but again, for compatibility reasons, this had to be updated. The game uses SmartFox as its server and client system.

## Website

Check out the [Exonaut Extended Support Wiki](https://projectexonaut.fandom.com/wiki/Exonaut_Extended_Support)