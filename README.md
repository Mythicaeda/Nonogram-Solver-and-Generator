# Nonogram Solver and Generator
A WinForms app that lets users to input the clues for a nonogram puzzle and, if the puzzle is solvable, to be shown a solution. Alternatively, a user can upload an image and dimensions and be shown a nonogram with that image as the solution.

## Generation
- Supports grayscale, square images.
- Valid puzzle dimensions range from 1x1 to 1000x1000 (or IMG_WIDTHxIMG_HEIGHT, whichever is smaller). Generated puzzles dont need to be square.
- Save generated puzzle as a .NONOGRAM file, so it can be uploaded to the Solving side.

## Solving
- Can upload a .NONOGRAM file or manually enter the row and column clues for a puzzle of dimensions 1x1 to 1000x1000.
- Validates a puzzle by checking that:
    - the length of a row/column doesn't exceed the maximum length available to it, 
    - the sum of the row clues equals the sum of the column clues, 
    - and that there is no immediate contradiction where there's a row clue of max length and a column clue of 0 (or vice versa).

Current task is implementing the solving algorithm.
