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

: interpretive{ ( -- wid )
    GET-CURRENT  interpretive-wordlist @ SET-CURRENT ;  

: }interpretive ( wid -- )
    SET-CURRENT ;

\ Make interpretive play with standard words that enter compilation and interpretation state
\ Redefine all Forth94 standard words that can change STATE, c.v 6.2.2250 and 15.6.2250

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

cr .( ready to define interpretive words )
cr .( usage: )
cr .(    interpretive{ )
cr .(      <interpretive_defintions> )
cr .(    }interpretive )
cr
