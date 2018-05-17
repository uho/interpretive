# interpretive

##  Standard Forth support for words with special interpretation and compilation semantics


On April, 26th 2018, Coherent Systems aka Bob Armstrong asked the question *Is there any "standard" way to make a state-smart tick , ' ?*:

> I find one of the most annoying "features" in Forth and painfully common gotcha 
> is that tick , ' , has to be changed to ['] in definitions . And this is also 
> a pain in trying to teach Forth to humans .
>
> I would like to move the Forth underlying CoSy ( http://CoSy.com ) towards
> a cleaner more "quotation" style concatenative language . For ordinary verbs , 
> what you write interactively , you should simply be able to "quote" , eg: 
> with { ... } , and have it work .
>
> Is there any more or less agreed upon "solution" to this issue ? Descending
> from the APL world , there's a lot of traditional Forth style I've bypassed .

What follows in comp.lang.forth is a lengthy discussion about the principles implied by the [Forth94]-standard's
notion of compilation and interpretation semantics. See the full comp.lang.forth [thread] for details.

Our solution:

    S" interpretive.fs" INCLUDED \ loads on any Forth94 standard system

    \ ticking words in interpretation and compilation state using the name xt-of

    : xt-of ( <name> -- )  POSTPONE ['] ; IMMEDIATE

    interpretive{
	
      : xt-of ( <name> -- xt )  ' ;
    
    }interpretive

    \ Usage of xt-of

    : .. ( x1 x2 -- ) \ execute . twice
      xt-of . DUP >R EXECUTE R> EXECUTE ;          \ compilation semantics of xt-of

    5 xt-of DUP EXECUTE ( 5 5 )  cr .. ( -> 5 5 )  \ interpretation semantics of xt-of

Most parts of the discussion in comp.lang.forth on this topic deals with (new) implementation techniques, that allow for words with independently defined *interpretation semantics* 
("The behavior of a Forth definition when its name is encountered by the text interpreter in interpretation state.", [Forth94], 2.1 Definitions of terms) 
and *compilation semantics* ("The behavior of a Forth definition when its name is encountered by the text interpreter in compilation state").
Eventually a word is executed and perfoms its *execution semantics* (i.e. The behavior of a Forth definition when it is executed.)

Traditionally Forth knows about two kinds of words:

- **normal words** are executed during interpretation and compiled during compilation.

- **immediate words** are executed during both interpretation and compilation.

[Forth94] specifies this behaviour in more detail. You have:

- **normal words**
    - They have an execution semantics according to their (colon) definition.  
    - Their interpretation semantics is to perform their execution semantics, i.e. execute the word.
    - Their compilation semantics is to compile code (into the current definition) so that the word's execution 
      semantics is performed if that code later runs. The community also calls this *default compilation semantics*<sup>1</sup> 
    - Examples for normal words are: ``DUP``, ``AND``, or ``ALLOT``.

- **immediate words** ("A Forth word whose compilation semantics are to perform its execution semantics.", [Forth94], 2.1 Definitions of terms)
    - They have an execution semantics according to their (colon) definition. Their immediacy is specified by using `IMMEDIATE` right after the word's definition.
    - Their interpretation semantices is to perform their execution semantics, i.e. execute the word.
    - Their compilation semantics is also to perform their execution semantics, i.e. execute the word.
    - Example for immediate words are ``(``, ``.(``, ``[`` or ``[IF]``.
    - [Forth94] however does not consider control structures or ``;`` to be immediate words and allows for other implementation techniques, so 

in addition [Forth94] words need not necessarily be immediate and can have divergent interpretation semantics and 
compilation semantics. As a special case their interpretation semantics could be left undefined (which means a compliant 
Forth system could react in a system specific<sup>2</sup> way when interpreting such a word, e.g. display an 
error message, abort the program, or perform the word's execution semantics<sup>3</sup>; a standard program could 
not rely an a certain system behaviour).
The community calls words with undefined interpretation semantics *compile-only* words and words with divergent interpretation and compilation semantics to have *non-default compilation semantics* (NDCS).  
Examples of words with undefined interpretation semantics are ``;``, ``IF``, ``."``, or ``SLITERAL``.  
Examples of words with divergent interpretation and compilation semantics are ``TO`` or ``S"``.  

<sup>1</sup>See [Forth94], 3.4.3.3 *Compilation Semantics* for the formal definition.  
<sup>2</sup>*Interpretating a word with undefined interpretation semantics* is an *ambiguous condition*, see [Forth94], 4.1.2 *Ambiguous conditions*  
<sup>3</sup>See [Forth94], 3.4.4 *Possible actions on an ambiguous condition*

---

Here is summary of the [Forth94] word kinds:

| word kind    | interpretation semantics                       | compilation semantics                       | example | comment
|--------------|------------------------------------------------|---------------------------------------------|---------|--------------------------------------
| *normal*     | perform execution semantics                    | compile execution semantics                 | DUP     | normal :-definitions
| immediate    | perform execution semantics                    | perform execution semantics                 | .(      | IMMEDIATE definitions
| compile-only | undefined                                      | perform execution semantics                 | IF      | interpretation semantics undefined
| NDCS         | perform execution semantics for interpretation | perform execution semantics for compilation | S"      | divergent interpretation, compilation

---

Traditionally NDCS-words were defined as immediate *state-smart* words that behave differently whether they are interpreted or compiled by inspecting the 
variable ``STATE`` at execution time of the state-smart word. This leads to inconsistent behaviour as pointed out by Anton Ertl in his Paper [*State-smartness-Why it is Evil and How to Exorcise it*][Ertl98].

There are several ways to implement NDCS words as Anton Ertl points out in the [comp.lang.forth discussion](https://groups.google.com/forum/#!original/comp.lang.forth/D5dc6cugT2o/dnKEzaI6CwAJ):

> There have been various implementations of this concept:
>
> a) cmForth uses dual wordlists, but this has other problems and did not catch on.
>
> b) Mark Humphries uses words with the same name in the same
>    wordlist, with flags that indicate whether the word implements
>    interpretation semantics only, compilation semantics only, a
>    normal word (where the compilation semantics is to COMPILE, the
>    interpretation semantics), or an immediate word (where the
>    compilation semantics is the same as the interpretation
>    semantics).  If you want to implement a word that has
>    non-default non-immediate compilation semantics (e.g., S"), you
>    write the interpretation semantics in an interpret-only word,
>    and the compilation semantics in a compile-only word of the same
>    name.
>
> c) Or you extend the header of a word so that you can get to the
>    interpretation semantics and the compilation semantics from the
>    header (Forth-2012 has NAME>INTERPRET and NAME>COMPILE for
>    that).  This has been implemented in Gforth in three different
>    ways over the years, and VFX 5.0 is announced to contain yet
>    another way to implement this idea.
>
>    You can find a general discussion of this issue and a
>    description of the first two Gforth implementations in
>    <http://www.euroforth.org/ef98/ertl98.pdf>,
>    and a description of the most recent implementation in
>    <https://wiki.forth-ev.de/lib/exe/fetch.php/events:ef2017:gforth-header.mp4>.
>    You can find a description of the VFX 5.0 implementation in
>    <http://www.euroforth.org/ef17/genproceedings/papers/pelc.pdf> 

## interpretive-wordlist

In the approach presented here the idea is to use a variation of a) to implement NDCS-words: define their 
interpretation semantics in the wordlist ``interpretive-wordlist`` and search this word list only when the text 
interpreter is in interpretation state.
If a name is found there, the text interpreter will perform its execution semantics.   
When in compilation state, ``interpretive-wordlist`` is not searched, the interactive words are not found and
their normal definition are used: Default compilation semantics on normal words and performing the execution semantics on
immediate words. All standard words that change between compilation state and interpretation state are redefined accordingly.
 
The advantage of this approach is that it uses only capabilities already existing in standard Forth94. No new features
need to be invented. Because of this it **easily loads on top of any Forth94 standard system**. Certainly if an 
application makes use of non-standard system specific features that interfere with our approach they need to be 
adapted in a system specific way.

In addition this approach can **easily be incorporated in any standard Forth94 system with word list support**:
Searching an additional word list while in interpretation mode is straight forward, a single line is all that is required:

    : interpreter ( i*x c-addr u -- j*x )
       2DUP interpretive-wordlist @ SEARCH-WORDLIST IF  NIP NIP  EXECUTE EXIT  THEN
       <normal interpreter definition>
    ;

### Syntactic sugar

Our point of view is that the compilation semantics is actually the relevant semantics for applications and that the
interpretation semantics is more or less syntactic sugar giving the programmer interactive convenience.

In order to define words with divergent interpretation semantics we use the form:

    interpretive{
	
	   <definitions for words with divergent interpretation semantics>
	
	}interpretive
	
`interpretive{` and `}interpretive` temporarily set `interpretice-wordlist` to be the current wordlist and revert to the previous current wordlist respectively. Other implementation techniques could be used with this notation.


### Ticking words 

If you want to access the execution token of a word you can distinguish between getting its interpretation xt by using 
the phrase `[ ' word ] LITERAL` and its compilation xt by using `[']`. For normal and immediate words both xts will be identical because they do not have a definition in `interpretive-wordlist`.

## Usage:

On a Forth94 standard system issue

    S" interpretive.fs" INCLUDED

to use this extension.

Have a look at the `example-*.fs` files for usage examples.

---

[Ertl98]:https://www.complang.tuwien.ac.at/anton/euroforth/ef98/ertl98.pdf
[thread]:https://groups.google.com/forum/#!topic/comp.lang.forth/D5dc6cugT2o%5B1-25%5D
[Forth94]:http://lars.nocrew.org/dpans/dpans.htm