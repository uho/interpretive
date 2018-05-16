\ ***********************************
\ ticking words
\ ***********************************

S" interpretive.fs" INCLUDED

\ make ['] also work in interpretive mode

interpretive{
    
: ['] ( <name> -- xt )  ' ;

}interpretive


\ define XT-OF that works in interpretation and compilation state

: xt-of ( <name> -- )  POSTPONE ['] ; IMMEDIATE

interpretive{
    
    : xt-of ( <name> -- xt )  ' ;

}interpretive


: .. ( x1 x2 -- )  xt-of . DUP >R EXECUTE R> EXECUTE ;

5 xt-of DUP EXECUTE  cr ..  ( -> 5 5 )
cr
