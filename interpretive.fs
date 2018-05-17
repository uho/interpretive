\ Have interpretive words as to avoid state smartness                                                       uh 2018-05-13

\ Provide the ability to define interactive words that are only found while the system is in interpretation mode.
\ This allows to define words that have divergent interpretation and compilation semantics to support the interpreter/compiler
\ copy&paste paradigm.
\
\ Usage Example:
\
\ Finding the execution token of a word ("ticking") in interpretation mode or in compilation mode with the same notation,
\ e.g.:
\
\ VARIABLE output    xt-of EMIT output ! 
\ : silent-output ( -- )  xt-of DROP  output ! ;
\
\ can be defined like this:
\
\ interpretive{
\
\   : xt-of ( <name> -- xt ) ' ;
\
\ }interpretive
\
\ : xt-of ( <name> -- )  POSTPONE ['] ; IMMEDIATE



\ -----------------------------------------------------------------------------------------------------------------------
\ Standard conformant labeling:
\ 
\ This is an ANS Forth Program with environmental dependencies, 
\    - Requiring ] [ VARIABLE THEN SWAP R> POSTPONE OVER IMMEDIATE IF DUP CR @ >R = ; : 1- 1+ 0= ! EVALUATE ( 
\        from the Core word set.
\    - Requiring NIP :NONAME .( \ from the Core Extensions word set.
\    - Requiring WORDS from the Programming-Tools word set.
\    - Requiring ;CODE from the Programming-Tools Extensions word set.
\    - Requiring WORDLIST SET-ORDER SET-CURRENT GET-ORDER GET-CURRENT from the Search-Order word set.
\ 
\ Required program documentation:
\ 
\    - Environmental dependencies
\      * This program has no known environmental dependencies.
\ 
\    - Other program documentation:
\      * After loading this program, a Standard System does no longer exist:
\           * in interpretation mode the first element in the search order is a word list with interactive words. 
\             Names in this wordlist are found first. In compilation mode, this wordlist is not the first wordlist 
\             in the search order so that ordinary definitions are found during compilation. Thus
\           * changing compilation state (by the words : ; ABORT QUIT :NONAME [ ]) also changes the search order,   
\             especially : changes the search order in contrast to the standardized behaviour of : in 6.1.0450. 
\             See also annex A.6.1.0450
\           * WORDS no longer displays the name of the first word list in the search order but avoids displaying
\             the interpretive word list.
\ -----------------------------------------------------------------------------------------------------------------------


\        0         0         0         0         0         0         0         0         0          1         1         1
\        1         2         3         4         5         6         7         8         9          0         1         2
\ 34567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890012345678901234567890


\ Where the interative words go

VARIABLE interpretive-wordlist   WORDLIST interpretive-wordlist !

: interpretive ( -- )
    GET-ORDER interpretive-wordlist @ SWAP 1+ SET-ORDER ;

: interpretive? ( -- f )
    GET-ORDER DUP IF  OVER interpretive-wordlist @ = >R SET-ORDER R>  THEN ;

: +interpretive ( -- )
    interpretive? 0= IF interpretive THEN ;

: -interpretive ( -- )
    interpretive? IF GET-ORDER NIP 1- SET-ORDER THEN ;

: interpretive{ ( -- wid )
    GET-CURRENT  interpretive-wordlist @ SET-CURRENT ;  

: }interpretive ( wid -- )
    SET-CURRENT ;

\ Make interpretive play with standard words that enter compilation and interpretation state
\ Redefine all Forth94 standard words that can change STATE, c.v 6.1.2250 and 15.6.2250

: [ ( -- )
    POSTPONE [  +interpretive ; IMMEDIATE

: ] ( -- )
    -interpretive ] ;

: : ( <name> -- )
    : -interpretive ;

: ; ( -- )
    POSTPONE ; +interpretive ; IMMEDIATE

: :NONAME ( -- xt )
    :NONAME -interpretive ;

: ;CODE ( -- )
    POSTPONE ;CODE +interpretive ; IMMEDIATE

: WORDS ( -- )
    GET-ORDER -interpretive WORDS SET-ORDER ;

: EVALUATE ( i*x c-addr u -- j*x )
   interpretive? >R +interpretive EVALUATE R> 0= IF -interpretive THEN ;

: ABORT ( -- )
   +interpretive ABORT ;
   
: ABORT" ( <ccc>" f -- )
   POSTPONE IF  [CHAR] " PARSE   POSTPONE SLITERAL  POSTPONE TYPE  POSTPONE ABORT  POSTPONE THEN ; IMMEDIATE

: QUIT ( -- )
   +interpretive QUIT ;

+interpretive

cr .( ready to define interpretive words )
cr .( usage: )
cr .(    interpretive{ )
cr .(      <interpretive_defintions> )
cr .(    }interpretive )
cr
