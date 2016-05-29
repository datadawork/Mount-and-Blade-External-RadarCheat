# Mount-and-Blade-External-RadarCheat

This was one of my first projects on the topic "game hacking". I knew very little about this topic 
so I used the internet to delve into a subject that is mostly taboo. Upon further research
it seemed best to go with C# and later down the line learn C++.

Going with C# for gamehacking comes with its complications. For one the .net framework is managed so you need to go with an external
library [MemorySharp](http://binarysharp.com/products/memorysharp/) in order to be able to read the games memory. I may plan to release my own
library for game hacking in C# but I make no promises as of now. C# also doesn't allow pointers since it is well... managed. You can
imagine how difficult it can be to retrieve some pointers if the language you are coding in doesn't support memory allocation. you can 
try circumvent this by using the [unsafe](https://msdn.microsoft.com/nl-be/library/chfa2zb8.aspx) keyword something I wasn't aware of at the time.

The current trend of today 5-3-2016 is to go front-end C# and back-end C++ combining both external and internal
ways of hacking so learning all this greatly improved my knowledge in memory manipulation. 

DISCLAIMER: this is made purely for an educational purpose. My desire never was to build "cheats"
but rather to create bots that would outsmart other players.

**This project got carried over in my new C++ PROJECT**
