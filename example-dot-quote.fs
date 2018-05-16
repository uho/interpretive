\ ************************************************************************************************INCLUDED
\ Interpretive ."
\ ************************************************************************************************

S" interpretive.fs" INCLUDED


interpretive{
    
: ." ( <ccc>" -- )  [CHAR] " PARSE TYPE ;

}interpretive

\ Sample usage:

: dq ( -- ) ." dot-quote." ;

cr ." An interpretive " dq cr  ( -> An interpretive dot-quote.)
cr
