\ ***********************************
\ S<  similar to S" 
\ ***********************************

S" interpretive.fs" INCLUDED

: s< ( <ccc>> -- ) [CHAR] > PARSE POSTPONE SLITERAL ; IMMEDIATE

interpretive{
  : s< ( <ccc>> -- c-addr u ) [CHAR] > PARSE ;
}interpretive


: msg  s< Hello, > type ;

msg s< interpretive world!> type    ( -> Hello, interpretive world!)
cr
