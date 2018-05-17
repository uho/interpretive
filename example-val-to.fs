\ ******************************************************
\ Interpretive Val -> +->  similar to  Value TO and +TO
\ ******************************************************

S" interpretive.fs" INCLUDED

: Val ( x -- )
    CREATE , DOES> @ ;

: -> ( <name> -- )
    ' >BODY POSTPONE LITERAL POSTPONE ! ; IMMEDIATE

interpretive{
    
  : -> ( x <name> -- ) ' >BODY ! ;

}interpretive

\ define increment operator +->
\ reuse -> as factor (no gotchas)  -- could be done easier otherwise - for illustration purposes
: +-> ( <name> -- )
    >IN @ >R  ' COMPILE,  POSTPONE +  R> >IN !  POSTPONE -> ; IMMEDIATE

interpretive{

  : +-> ( x <name> -- )  >IN @ >R ' EXECUTE  +  R> >IN !  [ ' -> ] LITERAL  EXECUTE ;

}interpretive

5 Val score

cr score .  ( -> 5 )

7 -> score  cr score . ( -> 7 )

: reset ( -- )
    0 -> score ;

reset  5 -> score   6 +-> score  cr score . ( -> 11 )

: inc ( -- )
    1 +-> score ;

inc  cr score .  ( -> 12 )
cr
