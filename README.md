fazfuck... U!! and fazassembly!!

# what
fazfuckU + fazass are two programming languages that combine to make fazfuck but better!
fazfuckU is a language that's intended to be more like a normal programming language, with if statements and functions and all the stuff they have. it then gets compiled (transpiled) into fazass, a bytecode-like language with simple functions so a developer can easily write an interpreter for it.

# why
usually, programming languages are designed with this order of least work to most work:
[end user] < [developer] (≈ [language/compiler creator])
but due to the language needing to be easy to use for the end user, the developer has to make all those complicated features the end users use like IF STATEMENTS!!! BLEGH!!! that's why fazass is designed with this order instead:
[developer] (< [language/compiler creator]) < [end user]
if the end user has enough motivation to make something for this thing the developer made, they should work for it!!
but the obvious problem is: who tf wants to write something in an assembly-byte-code-like language!? masochists BUT MOST PEOPLE AREN'T!

that's where fazfuckU comes in: it's intended to resemble a normal programming language so that the person who wants to make code can actually do that, and then get compiled into something the thing can actually run!! and in that way, it's primarily the compiler developer who has to go through all the pain. so ultimately we get this order:
[developer] (fazass) < [end user] (fazfuckU) < [language/compiler creator] (da compilah)
as you can see, the developer is still above the end user. fazfuckU can only do as much or less than fazass can due to having to be compiled to it to actually run. and since fazfuckU has the same access to functions fazass has, it's basically just a fancy way of writing fazass code!

# then what's it got
although fazass and especially fazfuckU are still in development, here are some features they will have. (*italic* means they are currently unimplements)
## both (these are directly converted from fazfuckU into their fazass equivalents)
- variables
 - most variables are global, and can be added at any point without needing a definition
 - global variables can be undefined if not needed anymore
 - *function*-local variables through use of the function stack
- *hooks*
 - *get ran through external functions; the main method programs communicate with the fazass/fazfuckU*
 - *can set global variables to send data*
 - *some hooks can be caught using catch, telling the program to not run some of it's own code*
## fazfuck
- human readable control flow
 - if, elif, and else blocks
 - *while loops with an optional otherwise block, which get ran if the condition isn't initially true*
- introduction of the new angle brackets!!! ⟨ ⟩
 - used to denote logic such as comparisons
 - make my job of writing the compiler easier
 - i just think they're neat
- *functions using the func keyword*
 - *local parameter variables as mentioned previously*
 - *supports out variables, so you can return values*
 - *use the inline keyword to have every call to the function get replaced with it in fazass, useful for simple/common functions to get preformance over size*
- the ; symbol can both seperate lines AND combine lines!
 - add a ; at the end of the line to continue it on the next
 - or add a ; between two functions to have them be compiled as two seperate lines

todo finish the read me