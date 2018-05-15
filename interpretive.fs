\ have intrepretive words as to avoid state smartness           uh 2018-05-13

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

: interpretive: ( -- )
    GET-CURRENT >R interpretive-wordlist @ SET-CURRENT : R> SET-CURRENT ;  \ ambigous condition to change current during definition 16.4.1.2, 16.3.3

\ An ambiguous condition exists if a program changes the compilation word list
\ during the compilation of a definition or before modification of the behavior
\ of the most recently compiled definition with ;CODE, DOES>, or IMMEDIATE.

: the ( <wordlist> <name> -- ) \ the interpretive word
    GET-ORDER  ' EXECUTE  ' COMPILE,  SET-ORDER ; IMMEDIATE  


\ get-current interpretive:
\ 
\ : hdjsh kflfjsd f ;
\ 
\ set-current


\ Make interpretive play with standard words that enter compilation and interpretation state

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

+interpretive


\ on error when going to  interpretive mode  interpretive-wordlist  should be established


\ ***********************************
\ Applications
\ ***********************************


\ Application: as an example define non-state smart ASCII as word with copy and paste capabilities



: (ascii) ( <ccc> -- x )
    BL WORD COUNT 0= IF -32 THROW ( invalid name argument ) THEN  C@ ;

: ASCII ( <ccc> -- )
    (ascii) POSTPONE LITERAL ; IMMEDIATE

interpretive: ASCII ( <ccc> -- x )
    (ascii) ;

\  Usage:

ASCII * emit

: star ( -- )  ASCII * emit ;


\ Define CTRL based on ascii

: (ctrl) ( <ccc> -- x )
    (ascii) [ BASE @ HEX ] 01F AND [ BASE ! ] ;

: CTRL ( <ccc> -- )
    (ctrl) POSTPONE LITERAL ; IMMEDIATE



interpretive: CTRL ( <ccc> -- x )
    (ctrl) ;


\ Usage:

CTRL C .

: .ETX ( -- )  CTRL C . ;


\ ***********************************

\ Interpretive ."

interpretive: ." ( <ccc>" -- )
    [CHAR] " PARSE TYPE ;



\ ***********************************

\ VAL and -> as  VALUE and TO pendant

: Val ( x -- )
    CREATE , DOES> @ ;

: -> ( <name> -- )
    ' >BODY POSTPONE LITERAL POSTPONE ! ; IMMEDIATE

interpretive: -> ( x <name> -- )
    ' >BODY ! ;


5 Val score

score .

7 -> score  score .

: reset ( -- )
    0 -> score ;

\ +->

\ resuse -> as factor (no gotchas)  -- could be done easier otherwise - for illustration purposes
: +-> ( <name> -- )
    >IN @ >R  ' COMPILE,  POSTPONE +  R> >IN !  POSTPONE -> ; IMMEDIATE

interpretive: +-> ( x <name> -- )
    >IN @ >R ' EXECUTE  +  R> >IN !  [ ' -> ] LITERAL  EXECUTE ;


reset  5 -> score   6 +-> score  score .

: inc ( -- )
    1 +-> score ;

inc  score .


\ ***********************************

\ Interpretive only version of s" xs   similar to ." and .(
: s( \ ( <ccc>) -- addr u )
    [CHAR] ) PARSE ; IMMEDIATE

: string-literal ( c -- )  PARSE POSTPONE SLITERAL ;

interpretive: s< ( <ccc>> -- c-addr u ) [CHAR] > PARSE ;

: s< ( <ccc>> -- )
\    the interpretive s<   POSTPONE SLITERAL 
    [CHAR] > string-literal
; IMMEDIATE


s< Test 123> type

: msg  s< abcd> type ;


\ ***********************************

\ ticking words

interpretive: ['] ( <name> -- xt )  ' ;


: xt-of ( <name> -- )  POSTPONE ['] ; IMMEDIATE

interpretive: xt-of ( <name> -- xt )  ' ;


5 xt-of DUP EXECUTE

: .. ( x1 x2 -- )  xt-of . DUP >R EXECUTE R> EXECUTE ;

cr ..

    

\ ***********************************

\ interpretive control structures


