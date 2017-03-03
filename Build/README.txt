------------------------------ REVERSI GAME MANUAL ------------------------------

                  ---------------- How to play ----------------
	   
	   
Reversi is played on an 8x8 gameboard. Each player starts with two pieces already
placed in the centre of the board. Players take turns placing pieces, starting
with Black.

                         -------- Placing pieces--------
						
Pieces are placed by clicking on an empty, valid tile. Valid tiles are those where
placing a piece will capture at least one enemy piece. Such tiles will be indicated
by the game with a grey circle.

If a player does not have any valid tiles available to him, he automatically passes
his turn. If neither player has any valid tiles available, the game ends.
						
                           -------- Capturing --------

Given the board below where W is White and B is Black, Black can capture the
white piece by placing a piece at B1.

             1   2   3
          A    |   |
            ---|---|---
          B    | W | B
            ---|---|---   
          C    |   |   
	  
All the white pieces between the two black pieces are converted to Black.
	  
             1   2   3
          A    |   |
            ---|---|---
          B  B | B | B
            ---|---|---   
          C    |   |   
	  
The line of white pieces can be up to six pieces long (board size of 8 and two
black pieces on either side), but there may not be an empty tile interrupting the line.

             1   2   3   4
          A    |   |   | 
            ---|---|---|---
          B  B | W |   |
            ---|---|---|---
          C    |   |   | W
            ---|---|---|---
          D    |   |   | B  
	  
In the above situation, placing a piece at B4 will capture the white piece at C4, but
will not capture the white piece at B2, as there is an empty tile at B3.

             1   2   3   4
          A    |   |   | 
            ---|---|---|---
          B  B | W |   | B
            ---|---|---|---
          C    |   |   | B
            ---|---|---|---
          D    |   |   | B  
	  

                        -------- End of the game --------
	  
When neither player has a valid tile available to him or the board is completely filled
(effectively causing the same), the game ends. At this point, the player who controls
the most tiles wins. If both players have the same amount of tiles, the game ends in a draw.
In this case, it is considered good sportsmanship to decide the victor by way of
another game.