\ ************************************************************************************************
\ character literals with ASCII, control characters with CTRL with copy and paste capabilities.
\ ************************************************************************************************

S" interpretive.fs" INCLUDED


: (ascii) ( <ccc> -- x )
    BL WORD COUNT 0= IF -32 THROW ( invalid name argument ) THEN  C@ ;

: ASCII ( <ccc> -- )
    (ascii) POSTPONE LITERAL ; IMMEDIATE

interpretive{
    
: ASCII ( <ccc> -- x )
   (ascii) ;

}interpretive


\ Define CTRL based on ascii

: (ctrl) ( <ccc> -- x )
    (ascii) [ BASE @ HEX ] 01F AND [ BASE ! ] ;  \ use (ascii) as factor

: CTRL ( <ccc> -- )
    (ctrl) POSTPONE LITERAL ; IMMEDIATE


interpretive{

: CTRL ( <ccc> -- x )
    (ctrl) ;

}interpretive

\ Sample Usage:

: star ( -- )  ASCII * emit ;

cr ASCII * emit  star  ( -> ** )

: .ETX ( -- )  CTRL C . ;
cr CTRL C .  .ETX  ( -> 3 3 )
cr

