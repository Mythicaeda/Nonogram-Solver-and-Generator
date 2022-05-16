using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace NonogramSolverGenerator
{
    class Solver
    {
        enum PuzzleStatus { Conflict, NoChange, Partial, Solved}

        //Byte value meanings
        //0 = unknown, 1 = empty, 2 = filled
        private readonly int rowLength;
        private readonly int colLength;
        private readonly string[] rowClues;
        private readonly string[] colClues;
        private readonly bool[][] completeRowClues;
        private readonly bool[][] completeColClues;
        //private int dictionaryKeyCtr = 0;

        //true means black, false means white in this dictionary
        //private SortedDictionary<int, (int, int)> orderedGuesses;
        private List<KeyValuePair<int, (int, int)>> orderedGuesses;

        private byte[][] puzzle;

        public Solver(List<string> rowClues, List<string> colClues)
        {
            //store the clues, then create the byte[][] initialized with unknowns 
            this.rowClues = rowClues.ToArray();
            this.colClues = colClues.ToArray();

            rowLength = this.colClues.Length;
            colLength = this.rowClues.Length;

            puzzle = new byte[this.rowClues.Length][];
            for(int i = 0; i < puzzle.Length; i++)
            {
                puzzle[i] = new byte[this.colClues.Length];
            }

            completeRowClues = new bool[this.rowClues.Length][];
            for(int i = 0; i < completeRowClues.Length; ++i) { completeRowClues[i] = new bool[this.rowClues[i].Split(' ').Length]; }

            completeColClues = new bool[this.colClues.Length][];
            for (int i = 0; i < completeColClues.Length; ++i) { completeColClues[i] = new bool[this.colClues[i].Split(' ').Length]; }

            //the guesses should now be ordered in descending order, which is what i want
            orderedGuesses = new List<KeyValuePair<int, (int, int)>>();//new SortedDictionary<int, (int, int)>(Comparer<int>.Create((x,y) => y.CompareTo(x)));           
        }

        //public bool Solve(BackgroundWorker bgw) => Crowding(CopyArray(puzzle)) == PuzzleStatus.Solved && !bgw.CancellationPending;

        private static byte[][] CopyArray(byte[][] source)
        {
            return source.Select(s => s.ToArray()).ToArray();
        }


        //Note: Explicitly need to check CancellationPending
        //That's why I've explicitly dragged bgw into this, so I can check its status inside the logic
        public bool Solve(BackgroundWorker bgw)
        {
            /* 
             * apply crowding, then
             * do
             *  Propogate:
             *      Find known Gaps (Guaranteed Spaces, Forcing, Splitting, Complete Line)
             *      Fill in more squares (Glue, Joining)
             *      Check if solved
             *      Check if changes were made
             * while(!cancelled && changes are happening)
             * else, Probe every unknown (as long as we're not cancelled)
             *  Propogate on if its black and if its white
             *  If both have a conflict, puzzle has a conflict
             *  If one has a conflict, the other is true
             *  If either is a solution, return it
             *  If both are plausible, commit their overlap and log how many addtional squares will be affected if we were to commit to a guess each way
             * else, Guess (aka a very odd A*)
             *  While(Guesses !empty, and the worker isn't cancelled):
             *      Apply the guess that will affect the most squares. Store that guess
             *      Propogate (we dont store the propogate result bc going through the whole technique might affect what the method will return)
             *      Check if solved
             *      Repeat. If we hit a conflict, rewind, take the other path 
             * Return Conflict
             */

            PuzzleStatus curStatus = Crowding(CopyArray(puzzle));
            if (curStatus == PuzzleStatus.Solved) { return !bgw.CancellationPending; }
            if (bgw.CancellationPending || curStatus == PuzzleStatus.Conflict) return false;

            do
            {
                curStatus = Propogate(ref this.puzzle);
            } while (!bgw.CancellationPending && curStatus == PuzzleStatus.Partial);

            if (bgw.CancellationPending || curStatus == PuzzleStatus.Conflict) return false;
            if (curStatus == PuzzleStatus.Solved) return !bgw.CancellationPending;

            //might have to update this measure
            for(int r = 0; !bgw.CancellationPending && r < puzzle.Length; r++)
            {
                for(int c = 0; c < puzzle[r].Length; c++)
                {
                    if(puzzle[r][c] == 0) //if the slot is still unknown
                    {
                        //Probe
                        byte[][] blackCheck = CopyArray(puzzle);
                        blackCheck[r][c] = 2;
                        byte[][] whiteCheck = CopyArray(puzzle);
                        whiteCheck[r][c] = 1;

                        PuzzleStatus blackResult = ValidatePuzzle(blackCheck);
                        PuzzleStatus whiteResult = ValidatePuzzle(whiteCheck);

                        Parallel.Invoke(
                            () => {
                                do
                                {
                                    blackResult = Propogate(ref blackCheck);
                                } while (!bgw.CancellationPending && blackResult == PuzzleStatus.Partial);
                            },
                            () => {
                                do
                                {
                                    whiteResult = Propogate(ref whiteCheck);
                                } while (!bgw.CancellationPending && whiteResult == PuzzleStatus.Partial);
                            }
                        );

                        if (bgw.CancellationPending||(blackResult == PuzzleStatus.Conflict && whiteResult == PuzzleStatus.Conflict)) { return false; }
                        
                        if(blackResult == PuzzleStatus.Solved) {
                            ApplyChanges(ref puzzle, blackCheck);
                            return !bgw.CancellationPending;
                        }
                        if(whiteResult == PuzzleStatus.Solved)
                        {
                            ApplyChanges(ref puzzle, whiteCheck);
                            return !bgw.CancellationPending;
                        }

                        if(blackResult == PuzzleStatus.Conflict) { ApplyChanges(ref puzzle, whiteCheck); }
                        else if (whiteResult == PuzzleStatus.Conflict) { ApplyChanges(ref puzzle, blackCheck); }
                        //If both are plausible, commit their overlap and log how many addtional squares will be affected if we were to commit the black guess
                        else { CommitOverlap(blackCheck, whiteCheck, r, c); }
                    }
                }
            }

            //If we hit here, start Guessing

            //but before we do, clean up OrderedGuesses, then sort it
            var toRemove = new List<KeyValuePair<int, (int, int)>>();
            foreach(KeyValuePair<int, (int, int)> kvp in orderedGuesses)
            {
                if(puzzle[kvp.Value.Item1][kvp.Value.Item2] != 0) { toRemove.Add(kvp); }
            }
            orderedGuesses.RemoveAll(n => toRemove.Contains(n));
            orderedGuesses.Sort(CompareGuesses);
            //now guess
            curStatus = Guess(ref puzzle, 0, bgw);

            if(curStatus == PuzzleStatus.Solved) { return !bgw.CancellationPending; }

            return false;//curStatus == PuzzleStatus.Solved && !bgw.CancellationPending;
        }

        private static int CompareGuesses(KeyValuePair<int, (int, int)> x, KeyValuePair<int, (int, int)> y)
        {
            //neither KVPs or ValueTuples can be null, so we bypass that check
            //doing y.compare to x because we want reverse sorting
            if(x.Key == y.Key)
            {
                if(x.Value.Item1 == y.Value.Item1) { return y.Value.Item2.CompareTo(x.Value.Item2); }
                return y.Value.Item1.CompareTo(x.Value.Item1);
            }
            return y.Key.CompareTo(x.Key);
        }

        private PuzzleStatus Guess(ref byte[][] puzzle, int ctr, BackgroundWorker bgw)
        {
            /*
             * While(Guesses !empty, and the worker isn't cancelled):
             * Apply the guess that will affect the most squares.Store that guess
             * Propogate(we dont store the propogate result bc going through the whole technique might affect what the method will return)
             *Check if solved
            * Repeat.If we hit a conflict, rewind, take the other path
            */

            if (bgw.CancellationPending) { return PuzzleStatus.Solved; } //solved gets us out of this faster
            if (orderedGuesses.Count <= ctr) { return PuzzleStatus.Conflict; }

            PuzzleStatus curStatus = PuzzleStatus.Conflict;
            
            var t = orderedGuesses.ElementAt(ctr).Value;
            byte[][] tempPuzzle = CopyArray(puzzle);
            if (tempPuzzle[t.Item1][t.Item2] == 0) { tempPuzzle[t.Item1][t.Item2] = 2; } //we'll try black first
            else { curStatus = PuzzleStatus.Conflict; }
            curStatus = ValidatePuzzle(tempPuzzle);

            while (curStatus != PuzzleStatus.Conflict)
            {
                do
                {
                    curStatus = Propogate(ref tempPuzzle);
                } while (curStatus == PuzzleStatus.Partial && !bgw.CancellationPending);
                if (curStatus == PuzzleStatus.Solved || bgw.CancellationPending)
                {
                    puzzle = tempPuzzle;
                    return PuzzleStatus.Solved;
                }
                if (curStatus != PuzzleStatus.Conflict) { curStatus = Guess(ref tempPuzzle, ctr + 1, bgw); }
            }


            //since an entry is only in orderedGuesses if both responses are plausible, also make white (needs to be done before we modify tempPuzzle)
            byte[][] oppPuzzle = CopyArray(puzzle);
            if (oppPuzzle[t.Item1][t.Item2] == 0) { oppPuzzle[t.Item1][t.Item2] = 1; }
            else { curStatus = PuzzleStatus.Conflict; }
            //now check the other side since the first side failed
            curStatus = ValidatePuzzle(oppPuzzle);

            while (curStatus != PuzzleStatus.Conflict)
            {
                do
                {
                    curStatus = Propogate(ref oppPuzzle);
                } while (curStatus == PuzzleStatus.Partial && !bgw.CancellationPending);
                if (curStatus == PuzzleStatus.Solved || bgw.CancellationPending)
                {
                    puzzle = oppPuzzle;
                    return PuzzleStatus.Solved;
                }
                if (curStatus != PuzzleStatus.Conflict) { curStatus = Guess(ref oppPuzzle, ctr + 1, bgw); }
            }

            return PuzzleStatus.Conflict;
        }

        private void CommitOverlap(byte[][] blackGuess, byte[][] whiteGuess, int rowIndex, int colIndex)
        {
            //this method also operates on the Solver's puzzle because it's only called from within the Solve() method
            int differenceCounter = 0;
            for(int r = 0; r < blackGuess.Length; ++r)
            {
                for(int c = 0; c< blackGuess[r].Length; ++c)
                {                    
                    if (blackGuess[r][c] == whiteGuess[r][c]) { this.puzzle[r][c] = blackGuess[r][c]; }
                    else { differenceCounter++; } // we don't have to check (puzzle[r][c] != blackGuess[r][c]) bc if they were equal, they would've tripped the if
                }
            }
            //update dictionary
            //TODO: figure out how to make unique keys (or refactor so that I'm using something that supports that
            //orderedGuesses.Add(differenceCounter, (rowIndex, colIndex));
            orderedGuesses.Add(new KeyValuePair<int, (int, int)>(differenceCounter, (rowIndex, colIndex)));
            //dictionaryKeyCtr++;
        }

        /// <summary>
        /// Apply crowding to rows and columns in seperate threads, then perform a merge. 
        /// </summary>
        /// <returns>
        /// Conflict, if the merge can't occur due to a conflict (ie attempting to mark a filled square as a known gap)
        /// Partial, if the puzzle is only partially solved
        /// Solved, if the puzzle is solved
        /// </returns>
        private PuzzleStatus Crowding(byte[][] puzzle)
        {
            byte[][] columnWise = CopyArray(puzzle);
            byte[][] rowWise = CopyArray(puzzle);

            //crowding
            Parallel.Invoke(
                () =>
                {
                    //rows
                    for (int i = 0; i < rowClues.Length; ++i)
                    {
                        //do the math check, then gather the indices that we care about and check those overlaps
                        var clues = rowClues[i].Split(' ');
                        int difFromRowLength = rowLength - clues.Select(n => int.Parse(n)).Sum() + clues.Length - 1;

                        if (difFromRowLength < 0) throw new ArgumentException("Clue sum is greater than row length.");

                        if (difFromRowLength == 0)
                        {
                            int startIndex = 0;
                            //this is a special case that means we actually don't need to do crowding
                            foreach (string str in clues)
                            {
                                int len = int.Parse(str);
                                for (int j = 0; j < len; ++j)
                                {
                                    //0 = unknown, 1 = empty, 2 = filled
                                    rowWise[i][startIndex + j] = 2;
                                }
                                startIndex += len;
                                if (startIndex < rowLength)
                                    rowWise[i][startIndex] = 1;
                            }
                        }
                        else if (difFromRowLength == rowLength)
                        {
                            //we know that clues = 0, so slap a row of gaps down
                            for (int j = 0; j < rowLength; ++j)
                            {
                                rowWise[i][j] = 1;
                            }
                        }
                        else
                        {

                            //else let's do some crowding
                            //and by crowding i mean that there will be overlap from left edge - difFromRowLength

                            //the routine here is jump ahead the dif, then fill in the overlap, then jump ahead the gap
                            //if a clue is too short, jump ahead the length of the clue + 1 for the gap
                            int startingIndex = 0;
                            for (int j = 0; j < clues.Length; j++)
                            {
                                int len = int.Parse(clues[j]);
                                if (len - difFromRowLength > 0)
                                {
                                    startingIndex += difFromRowLength;
                                    for (int k = 0; k < len - difFromRowLength; ++k)
                                    {
                                        rowWise[i][startingIndex] = 2;
                                        startingIndex++;
                                    }
                                }
                                else
                                {
                                    startingIndex += len + 1;
                                }
                            }
                        }
                    }
                },
                () =>
                {
                    //cols is the same as rows, but refers to different fields
                    for (int i = 0; i < colClues.Length; ++i)
                    {
                        var clues = colClues[i].Split(' ');
                        int difFromColLength = colLength - clues.Length + 1;
                        //- clues.Select(n => int.Parse(n)).Sum() ;
                        foreach(string clue in clues) { difFromColLength -= int.Parse(clue); }

                        if (difFromColLength < 0) throw new ArgumentException("Clue sum is greater than col length.");

                        if (difFromColLength == 0)
                        {
                            int startIndex = 0;
                            //this is a special case that means we actually don't need to do crowding
                            foreach (string str in clues)
                            {
                                int len = int.Parse(str);
                                for (int j = 0; j < len; ++j)
                                {
                                    //0 = unknown, 1 = empty, 2 = filled
                                    columnWise[startIndex + j][i] = 2;
                                }
                                startIndex += len;
                                if (startIndex < colLength)
                                {
                                    columnWise[startIndex][i] = 1;
                                    startIndex++;
                                }
                            }
                        }
                        else if (difFromColLength == colLength)
                        {
                            //we know that clues = 0, so slap a row of gaps down
                            for (int j = 0; j < colLength; ++j)
                            {
                                columnWise[j][i] = 1;
                            }
                        }
                        else
                        {
                            int startingIndex = 0;
                            for (int j = 0; j < clues.Length; j++)
                            {
                                int len = int.Parse(clues[j]);
                                if (len - difFromColLength > 0)
                                {
                                    startingIndex += difFromColLength;
                                    for (int k = 0; k < len - difFromColLength; ++k)
                                    {
                                        columnWise[startingIndex][i] = 2;
                                        startingIndex++;
                                    }
                                    startingIndex++;
                                }
                                else
                                {
                                    startingIndex += len + 1;
                                }
                            }
                        }
                    }

                });

            //merge
            if (!Merge(rowWise, columnWise, ref puzzle)) { return PuzzleStatus.Conflict; }
            //validate
            PuzzleStatus validationStatus = ValidatePuzzle(puzzle);
            if (validationStatus == PuzzleStatus.Conflict) return PuzzleStatus.Conflict;
            return !ApplyChanges(ref this.puzzle, puzzle) ? PuzzleStatus.NoChange : validationStatus;
        }

        private PuzzleStatus Propogate(ref byte[][] original)
        {
            /*
             * Propogate:
             *      Find known Gaps (Guaranteed Spaces, Forcing, Splitting, Complete Line)
             *      Fill in more squares (Glue, Joining)
             *      Check if solved
             *      Check if changes were made
             */
            byte[][] working = CopyArray(original);
            //---Find Gaps---//
            PuzzleStatus curStatus = GuaranteedSpaces(ref working);
            if(curStatus == PuzzleStatus.Solved || curStatus == PuzzleStatus.Conflict) { return curStatus; }

            PuzzleStatus newStatus = Splitting(ref working);
            //update cur status
            if (newStatus == PuzzleStatus.Conflict||newStatus == PuzzleStatus.Solved) return newStatus;
            if(curStatus == PuzzleStatus.NoChange && newStatus == PuzzleStatus.Partial) { curStatus = newStatus; }
            
            //---Find Squares---//
            newStatus = Glue(ref working);
            //update cur status
            if (newStatus == PuzzleStatus.Conflict || newStatus == PuzzleStatus.Solved) return newStatus;
            if (curStatus == PuzzleStatus.NoChange && newStatus == PuzzleStatus.Partial) { curStatus = newStatus; }
            /*
            newStatus = Joining(ref working);
            //update curStatus
            if (newStatus == PuzzleStatus.Conflict || newStatus == PuzzleStatus.Solved) return newStatus;
            if (curStatus == PuzzleStatus.NoChange && newStatus == PuzzleStatus.Partial) { curStatus = newStatus; }
            */
            //If changes were made, Validate
            if (curStatus == PuzzleStatus.NoChange) return PuzzleStatus.NoChange;
            //update the puzzle
            newStatus = ValidatePuzzle(working);

            if(newStatus != PuzzleStatus.Conflict)
            {
                original = working;
                return newStatus;
            }

            return PuzzleStatus.Conflict;
            

            //return curStatus == PuzzleStatus.NoChange? curStatus: ValidatePuzzle(working);
        }

        /* 
         * this needs to apply the following:
         *      If the line is completed, everything else is a gap
         *      Completed Clues have a gap on either side
         *      Any spaces that cant be reached by the clues are gaps <- will come back to represent this, bc at least guessing will failsafe this
        */
        private PuzzleStatus GuaranteedSpaces(ref byte[][] puzzle)
        {
            bool conflict = false;
            byte[][] rowWise = CopyArray(puzzle);
            byte[][] colWise = CopyArray(puzzle);

            Parallel.Invoke(
                () => {
                    for (int ri = 0; !conflict && ri < rowClues.Length; ++ri)
                    {
                        int rowSum = rowClues[ri].Split(' ').Select(n => int.Parse(n)).Sum();
                        int curSum = rowWise[ri].Where(n => n > 0).Select(n => n - 1).Sum();

                        //---A Line is Complete---//
                        if (curSum == rowSum)
                        {
                            for (int i = 0; i < rowLength; ++i)
                            {
                                if (rowWise[ri][i] != 2) { rowWise[ri][i] = 1; }
                            }
                        }
                        else if (curSum > rowSum) { conflict = true; }
                        //---Complete Clues Have a Gap on either side---//
                        else
                        {
                            var clues = rowClues[ri].Split(' ');
                            int clueCounter = 0;
                            int blockLenCtr = 0;
                            bool clueMatch = true;

                            //loop forwards through this specific row
                            for (int i = 0; clueMatch && i < rowLength; i++)
                            {
                                if (rowWise[ri][i] == 2)
                                {
                                    blockLenCtr++;
                                    //if (i > 0 && blockLenCtr == 1) { rowWise[ri][i - 1] = 1; } //this is to put the gap on the left side
                                }
                                else
                                {
                                    if (blockLenCtr > 0)
                                    {
                                        //if we just went across one whole clue
                                        if (clueCounter < clues.Length && blockLenCtr == int.Parse(clues[clueCounter]))
                                        {
                                            blockLenCtr = 0;
                                            clueCounter++;
                                            rowWise[ri][i] = 1;
                                            /*if (i + 1 < rowLength) //gap on the right side
                                            {
                                                //i++; //we dont have to look at the next square b/c we're setting it right now
                                                rowWise[ri][i] = 1;
                                            }*/
                                        }
                                        else if (blockLenCtr > int.Parse(clues[clueCounter])) { conflict = true; } //if we exceeded, then something bad happened
                                        else
                                        {
                                            clueMatch = false;
                                        }
                                    }
                                }
                            }

                            //loop backwards through this row
                            clueMatch = true;
                            clueCounter = clues.Length - 1;
                            blockLenCtr = 0;
                            for (int i = rowLength - 1; clueMatch && i >= 0; --i)
                            {
                                if (rowWise[ri][i] == 2)
                                {
                                    blockLenCtr++;
                                    //if (i < rowLength - 1 && blockLenCtr == 1) { rowWise[ri][i + 1] = 1; } //this is to put the gap on the right side
                                }
                                else
                                {
                                    if (blockLenCtr > 0)
                                    {
                                        //if we just went across one whole clue
                                        if (clueCounter >= 0 && blockLenCtr == int.Parse(clues[clueCounter]))
                                        {
                                            blockLenCtr = 0;
                                            clueCounter--;
                                            rowWise[ri][i] = 1;
                                            /*if (i - 1 >= 0) //gap on the left side
                                            {
                                                i--; //we dont have to look at the next square b/c we're setting it right now
                                                rowWise[ri][i] = 1;
                                            }*/
                                        }
                                        else if (blockLenCtr > int.Parse(clues[clueCounter])) { conflict = true; } //if we exceeded, then something bad happened
                                        else
                                        {
                                            clueMatch = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                () => {
                    for (int ci = 0; !conflict && ci < colClues.Length; ++ci)
                    {
                        int colSum = colClues[ci].Split(' ').Select(n => int.Parse(n)).Sum();
                        int curSum = 0;
                        for (int r = 0; r < rowClues.Length; r++)
                        {
                            if (colWise[r][ci] == 2) { curSum++; }
                        }

                        //---A Line is Complete---//
                        if (curSum == colSum)
                        {
                            for (int i = 0; i < colLength; ++i)
                            {
                                if (colWise[i][ci] != 2) { colWise[i][ci] = 1; }
                            }
                        }
                        else if (curSum > colSum) { conflict = true; }
                        //---Complete Clues Have a Gap on either side---//
                        else
                        {
                            var clues = colClues[ci].Split(' ');
                            int clueCounter = 0;
                            int blockLenCtr = 0;
                            bool clueMatch = true;

                            //loop forwards through this specific column
                            for (int i = 0; clueMatch && i < colLength; i++)
                            {
                                if (colWise[i][ci] == 2)
                                {
                                    blockLenCtr++;
                                    //if (i > 0 && blockLenCtr == 1) { colWise[i-1][ci] = 1; } //this is to put the gap above
                                }
                                else
                                {
                                    if (blockLenCtr > 0)
                                    {
                                        //if we just went across one whole clue
                                        if (clueCounter < clues.Length && blockLenCtr == int.Parse(clues[clueCounter]))
                                        {
                                            blockLenCtr = 0;
                                            clueCounter++;
                                            colWise[i][ci] = 1;
                                            /*if (i + 1 < colLength) //gap below
                                            {
                                                i++; //we dont have to look at the next square b/c we're setting it right now
                                                colWise[i][ci] = 1;
                                            }*/
                                        }
                                        else if (blockLenCtr > int.Parse(clues[clueCounter])) { conflict = true; } //if we exceeded, then something bad happened
                                        else
                                        {
                                            clueMatch = false;
                                        }
                                    }
                                }
                            }

                            //loop backwards through this column
                            clueMatch = true;
                            clueCounter = clues.Length - 1;
                            blockLenCtr = 0;
                            for (int i = colLength - 1; clueMatch && i >= 0; --i)
                            {
                                if (colWise[i][ci] == 2)
                                {
                                    blockLenCtr++;
                                    //if (i < colLength - 1 && blockLenCtr == 1) { colWise[i+1][ci] = 1; } //this is to put the gap below
                                }
                                else
                                {
                                    if (blockLenCtr > 0)
                                    {
                                        //if we just went across one whole clue
                                        if (clueCounter >= 0 && blockLenCtr == int.Parse(clues[clueCounter]))
                                        {
                                            blockLenCtr = 0;
                                            clueCounter--;
                                            colWise[i][ci] = 1;
                                            /*if (i - 1 >= 0) //gap above
                                            {
                                                i--; //we dont have to look at the next square b/c we're setting it right now
                                                colWise[i][ci] = 1;
                                            }*/
                                        }
                                        else if (blockLenCtr > int.Parse(clues[clueCounter])) { conflict = true; } //if we exceeded, then something bad happened
                                        else
                                        {
                                            clueMatch = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                );

            return Update(ref puzzle, rowWise, colWise);
        }

        

        private PuzzleStatus Splitting(ref byte[][] puzzle) //maybe convert to bool: changes made y/n?
        {
            //LOGIC MISTAKE HERE: A SERIES OF UNKNOWNS EQUAL TO THE CURRENT CLUE MEANS WE DONT KNOW WHAT'S GOING ON HERE

            //this is just looking at unknowns between two blocks and, if putting a square there would make the block too large, it must be a gap
            byte[][] rowWise = CopyArray(puzzle);
            byte[][] colWise = CopyArray(puzzle);

            Parallel.Invoke(
                () => {
                    for (int ri = 0; ri < rowClues.Length; ++ri)
                    {
                        var clues = rowClues[ri].Split(' ').Select(n => int.Parse(n)).ToArray();

                        //var clues = rowClues[ri].Split(' ');
                        int clueCounter = 0;
                        int blockLenCtr = 0;
                        //loop forwards through this specific row
                        for (int i = 0; i < rowLength && clueCounter < clues.Length; i++)
                        {
                            if (rowWise[ri][i] == 2)
                            {
                                blockLenCtr++;
                            }
                            else if(rowWise[ri][i] == 1)
                            {
                                if (blockLenCtr > 0)
                                {
                                    blockLenCtr = 0;
                                    //this is a gap, so we know we just advanced a clue
                                    clueCounter++;
                                }
                            }
                            else
                            {
                                if(blockLenCtr > 0)
                                {
                                    //if we're at an unknown, see if the next square is black
                                    //if it is, get the length of the upcoming block. if it + the current block is greater than the current clue, put a gap
                                    if (i + 1< rowLength && rowWise[ri][i + 1] == 2)
                                    {
                                        int upcomingBlockLen = 0;
                                        for (int j = i + 1; j < rowLength; ++j)
                                        {
                                            if (rowWise[ri][j] != 2) break;
                                            upcomingBlockLen++;
                                        }

                                        if(upcomingBlockLen + blockLenCtr > clues[clueCounter])
                                        {
                                            rowWise[ri][i] = 1;
                                            clueCounter++;//we just slapped a gap down, so we KNOW we're onto a new clue
                                        }
                                    }
                                    //advance until we know we've exceeded the clue
                                    if(clueCounter >= clues.Length) { break; }
                                    for(int k = blockLenCtr; k < clues[clueCounter] && i+k < rowLength; ++k)
                                    {
                                        if(rowWise[ri][i+k] == 2)
                                        {
                                            int upcomingBlockLen = 0;
                                            for (int j = i+k; j < rowLength; ++j)
                                            {
                                                if (rowWise[ri][j] != 2) break;
                                                upcomingBlockLen++;
                                            }
                                            if (upcomingBlockLen + blockLenCtr + (k - blockLenCtr) > clues[clueCounter])
                                            {
                                                rowWise[ri][i+k-1] = 1;
                                                clueCounter++;//we just slapped a gap down, so we KNOW we're onto a new clue
                                                break;
                                            }
                                        }
                                        else if(k == clues[clueCounter] - 1) { clueCounter++; break; } //we've gone the rest of the clue from the last black square
                                    }

                                    blockLenCtr = 0;
                                }
                            }
                        }

                        //loop backwards through this row
                        clueCounter = clues.Length-1;
                        blockLenCtr = 0;
                        for (int i = rowLength-1; i >= 0 && clueCounter >= 0; --i)
                        {
                            if (rowWise[ri][i] == 2)
                            {
                                blockLenCtr++;
                            }
                            else if (rowWise[ri][i] == 1)
                            {
                                if (blockLenCtr > 0)
                                {
                                    blockLenCtr = 0;
                                    //this is a gap, so we know we just advanced a clue
                                    clueCounter--;
                                }
                            }
                            else
                            {
                                if (blockLenCtr > 0)
                                {
                                    //if we're at an unknown, see if the next square is black
                                    //if it is, get the length of the upcoming block. if it + the current block is greater than the current clue, put a gap
                                    if (i - 1 > 0 && rowWise[ri][i - 1] == 2)
                                    {
                                        int upcomingBlockLen = 0;
                                        for (int j = i - 1; j >= 0; ++j)
                                        {
                                            if (rowWise[ri][j] != 2) break;
                                            upcomingBlockLen++;
                                        }

                                        if (upcomingBlockLen + blockLenCtr > clues[clueCounter])
                                        {
                                            rowWise[ri][i] = 1;
                                            clueCounter--;//we just slapped a gap down, so we KNOW we're onto a new clue
                                        }
                                    }
                                    //advance until we know we've exceeded the clue
                                    if (clueCounter < 0) { break; }
                                    for (int k = blockLenCtr; k < clues[clueCounter] && i - k >= 0; ++k)
                                    {
                                        if (rowWise[ri][i-k] == 2)
                                        {
                                            int upcomingBlockLen = 0;
                                            for (int j = i-k; j >= 0; ++j)
                                            {
                                                if (rowWise[ri][j] != 2) break;
                                                upcomingBlockLen++;
                                            }
                                            if (upcomingBlockLen + blockLenCtr + (k - blockLenCtr) > clues[clueCounter])
                                            {
                                                rowWise[ri][i - k + 1] = 1;
                                                clueCounter++;//we just slapped a gap down, so we KNOW we're onto a new clue
                                                break;
                                            }
                                        }
                                        else if (k == clues[clueCounter] - 1) { clueCounter--; i -= k;  break; } //we've gone the rest of the clue from the last black square
                                    }
                                    blockLenCtr = 0;
                                }
                            }
                        }
                    }
                },
                () => {
                    for (int ci = 0; ci < colClues.Length; ++ci)
                    {
                        var clues = colClues[ci].Split(' ').Select(n => int.Parse(n)).ToArray();

                        //var clues = rowClues[ri].Split(' ');
                        int clueCounter = 0;
                        int blockLenCtr = 0;
                        //loop forwards through this specific col
                        for (int i = 0; i < colLength && clueCounter < clues.Length; i++)
                        {
                            if (colWise[i][ci] == 2)
                            {
                                blockLenCtr++;
                            }
                            else if (colWise[i][ci] == 1)
                            {
                                if (blockLenCtr > 0)
                                {
                                    blockLenCtr = 0;
                                    //this is a gap, so we know we just advanced a clue
                                    clueCounter++;
                                }
                            }
                            else
                            {
                                if (blockLenCtr > 0)
                                {
                                    //if we're at an unknown, see if the next square is black
                                    //if it is, get the length of the upcoming block. if it + the current block is greater than the current clue, put a gap
                                    if (i + 1< colLength && colWise[i + 1][ci] == 2)
                                    {
                                        int upcomingBlockLen = 0;
                                        for (int j = i + 1; j < colLength; ++j)
                                        {
                                            if (colWise[j][ci] != 2) break;
                                            upcomingBlockLen++;
                                        }

                                        if (upcomingBlockLen + blockLenCtr > clues[clueCounter])
                                        {
                                            colWise[i][ci] = 1;
                                            clueCounter++;//we just slapped a gap down, so we KNOW we're onto a new clue
                                        }
                                    }
                                    //advance until we know we've exceeded the clue
                                    if (clueCounter >= clues.Length) { break; }
                                    for (int k = blockLenCtr; k < clues[clueCounter] && i + k < colLength; ++k)
                                    {
                                        if (colWise[i + k][ci] == 2)
                                        {
                                            int upcomingBlockLen = 0;
                                            for (int j = i + k; j < colLength; ++j)
                                            {
                                                if (colWise[j][ci] != 2) break;
                                                upcomingBlockLen++;
                                            }
                                            if (upcomingBlockLen + blockLenCtr + (k - blockLenCtr) > clues[clueCounter])
                                            {
                                                colWise[i + k - 1][ci] = 1;
                                                clueCounter++;//we just slapped a gap down, so we KNOW we're onto a new clue
                                                break;
                                            }
                                        }
                                        else if (k == clues[clueCounter] - 1) { clueCounter++; break; } //we've gone the rest of the clue from the last black square
                                    }

                                    blockLenCtr = 0;
                                }
                            }
                        }

                        //loop backwards through this col
                        clueCounter = clues.Length - 1;
                        blockLenCtr = 0;
                        for (int i = colLength - 1; i >= 0 && clueCounter >= 0; --i)
                        {
                            if (colWise[i][ci] == 2)
                            {
                                blockLenCtr++;
                            }
                            else if (colWise[i][ci] == 1)
                            {
                                if (blockLenCtr > 0)
                                {
                                    blockLenCtr = 0;
                                    //this is a gap, so we know we just advanced a clue
                                    clueCounter--;
                                }
                            }
                            else
                            {
                                if (blockLenCtr > 0)
                                {
                                    //if we're at an unknown, see if the next square is black
                                    //if it is, get the length of the upcoming block. if it + the current block is greater than the current clue, put a gap
                                    if (i - 1 > 0 && colWise[i - 1][ci] == 2)
                                    {
                                        int upcomingBlockLen = 0;
                                        for (int j = i - 1; j >= 0; ++j)
                                        {
                                            if (colWise[j][ci] != 2) break;
                                            upcomingBlockLen++;
                                        }

                                        if (upcomingBlockLen + blockLenCtr > clues[clueCounter])
                                        {
                                            colWise[i][ci] = 1;
                                            clueCounter--;//we just slapped a gap down, so we KNOW we're onto a new clue
                                        }
                                    }
                                    //advance until we know we've exceeded the clue
                                    if (clueCounter < 0) { break; }
                                    for (int k = blockLenCtr; k < clues[clueCounter] && i - k >= 0; ++k)
                                    {
                                        if (colWise[i - k][ci] == 2)
                                        {
                                            int upcomingBlockLen = 0;
                                            for (int j = i - k; j >= 0; ++j)
                                            {
                                                if (colWise[j][ci] != 2) break;
                                                upcomingBlockLen++;
                                            }
                                            if (upcomingBlockLen + blockLenCtr + (k - blockLenCtr) > clues[clueCounter])
                                            {
                                                colWise[i - k + 1][ci] = 1;
                                                clueCounter++;//we just slapped a gap down, so we KNOW we're onto a new clue
                                                break;
                                            }
                                        }
                                        else if (k == clues[clueCounter] - 1) { clueCounter--; i -= k; break; } //we've gone the rest of the clue from the last black square
                                    }
                                    blockLenCtr = 0;
                                }
                            }
                        }
                    }
                }
            );

            return Update(ref puzzle, rowWise, colWise);
        }

        //somewhat like crowding, but working on a smaller scale
        private PuzzleStatus Glue(ref byte[][] puzzle)
        {
            byte[][] rowWise = CopyArray(puzzle);
            byte[][] colWise = CopyArray(puzzle);

            Parallel.Invoke(
                () => 
                {
                    for (int ri = 0; ri < rowClues.Length; ++ri)
                    {
                        int rowSum = rowClues[ri].Split(' ').Select(n => int.Parse(n)).Sum();
                        int curSum = rowWise[ri].Where(n => n > 0).Select(n => n - 1).Sum();
                        if(rowSum == curSum) { continue; }

                        int[] clues = rowClues[ri].Split(' ').Select(n => int.Parse(n)).ToArray();

                        int clueCounter = 0;
                        int lastGapPos = 0;
                        int blockStartPos = 0;
                        int curBlockLen = 0;

                        //forwards loop
                        for (int i = 0; i < rowLength && clueCounter < clues.Length; ++i)
                        {
                            if (rowWise[ri][i] == 2)
                            {
                                if (curBlockLen == 0) { blockStartPos = i; }
                                if (lastGapPos == i - 1)
                                {
                                    //fill in the rest of the clue if this is following off a known gap
                                    int j = 0;
                                    for (; j < clues[clueCounter] && blockStartPos + j < rowLength && rowWise[ri][blockStartPos + j] != 1; ++j)
                                    {
                                        rowWise[ri][blockStartPos + j] = 2;
                                    }
                                    j++;
                                    //if (blockStartPos + j < rowLength) { rowWise[ri][blockStartPos + j] = 1; }
                                    i += j-1;
                                    curBlockLen = 0;
                                    clueCounter++;
                                    continue;
                                }                              
                                curBlockLen++;
                            }
                            else if (rowWise[ri][i] == 1)
                            {
                                lastGapPos = i;
                                //check if we've completed the clue
                                if (curBlockLen > 0)// && curBlockLen == clues[clueCounter])
                                {
                                    clueCounter++;
                                    curBlockLen = 0;
                                }
                            }
                            else if (rowWise[ri][i] == 0 && curBlockLen > 0 && clueCounter < clues.Length)
                            {
                                //derived from the mathematical approach used in Crowding
                                int excess = clues[clueCounter] - blockStartPos - lastGapPos - curBlockLen;
                                if (excess > 0)
                                {
                                    int j = 0;
                                    for (; j < excess && j + blockStartPos < rowLength && rowWise[ri][j + blockStartPos] != 1; ++j)
                                    {
                                        rowWise[ri][j + blockStartPos] = 2;
                                    }
                                    i += j;
                                    curBlockLen = 0;
                                    clueCounter++;
                                }
                            }
                        }

                        clueCounter = clues.Length - 1;
                        lastGapPos = rowLength - 1;
                        blockStartPos = rowLength - 1;
                        curBlockLen = 0;

                        //backwards loop
                        for (int i = rowLength - 1; i >= 0 && clueCounter >= 0; --i)
                        {
                            if (rowWise[ri][i] == 2)
                            {
                                if (curBlockLen == 0) { blockStartPos = i; }
                                if (lastGapPos == i + 1)
                                {
                                    //fill in the rest of the clue if this is following off a known gap
                                    int j = 0;
                                    for (; j < clues[clueCounter] && blockStartPos - j >= 0 && rowWise[ri][blockStartPos - j] != 1; ++j)
                                    {
                                        rowWise[ri][blockStartPos - j] = 2;
                                    }
                                    j++;
                                    //if (blockStartPos - j >= 0) { rowWise[ri][blockStartPos - j] = 1; }
                                    i -= j+1;
                                    curBlockLen = 0;
                                    clueCounter--;
                                    continue;
                                }
                                curBlockLen++;
                            }
                            else if (rowWise[ri][i] == 1)
                            {
                                lastGapPos = i;
                                //check if we've completed the clue
                                if (curBlockLen > 0 )//&& curBlockLen == clues[clueCounter])
                                {
                                    clueCounter--;
                                    curBlockLen = 0;
                                }
                            }
                            else if (rowWise[ri][i] == 0 && curBlockLen > 0 && clueCounter >= 0)
                            {
                                //derived from the mathematical approach used in Crowding
                                int excess = clues[clueCounter] - (rowLength - lastGapPos) - (rowLength - blockStartPos) - curBlockLen;
                                if (excess > 0)
                                {
                                    int j = 0;
                                    for (; j < excess && blockStartPos - j >= 0 && rowWise[ri][blockStartPos - j] != 1; ++j)
                                    {
                                        rowWise[ri][blockStartPos - j] = 2;
                                    }
                                    i -= j;
                                    curBlockLen = 0;
                                    clueCounter--;
                                }
                            }
                        }
                    }
                }, 
                () => 
                {
                    for (int ci = 0; ci < colClues.Length; ++ci)
                    {
                        int colSum = colClues[ci].Split(' ').Select(n => int.Parse(n)).Sum();
                        int curSum = 0;
                        for (int r = 0; r < rowClues.Length; r++)
                        {
                            if (colWise[r][ci] == 2) { curSum++; }
                        }
                        if (curSum == colSum) { continue; }


                        int[] clues = colClues[ci].Split(' ').Select(n => int.Parse(n)).ToArray();

                        int clueCounter = 0;
                        int lastGapPos = 0;
                        int blockStartPos = 0;
                        int curBlockLen = 0;

                        //forwards loop
                        for (int i = 0; i < colLength && clueCounter < clues.Length; ++i)
                        {
                            if (colWise[i][ci] == 2)
                            {
                                if (curBlockLen == 0) { blockStartPos = i; }
                                if (lastGapPos == i - 1)
                                {
                                    //fill in the rest of the clue if this is following off a known gap
                                    int j = 0;
                                    for (; j < clues[clueCounter] && blockStartPos + j < colLength && colWise[blockStartPos + j][ci] != 1; ++j)
                                    {
                                        colWise[blockStartPos + j][ci] = 2;
                                    }
                                    j++;
                                    //if (blockStartPos + j < colLength) { colWise[blockStartPos + j][ci] = 1; }
                                    i += j-1;
                                    curBlockLen = 0;
                                    clueCounter++;
                                    continue;
                                }
                                curBlockLen++;
                            }
                            else if (colWise[i][ci] == 1)
                            {
                                lastGapPos = i;
                                //check if we've completed the clue
                                if (curBlockLen > 0)// && curBlockLen == clues[clueCounter])
                                {
                                    clueCounter++;
                                    curBlockLen = 0;
                                }
                            }
                            else if (colWise[i][ci] == 0 && curBlockLen > 0 && clueCounter < clues.Length)
                            {
                                //derived from the mathematical approach used in Crowding
                                int excess = clues[clueCounter] - blockStartPos - lastGapPos - curBlockLen;
                                if (excess > 0)
                                {
                                    int j = 0;
                                    for (; j < excess && j + blockStartPos < colLength && colWise[j+blockStartPos][ci] != 1; ++j)
                                    {
                                        colWise[j + blockStartPos][ci] = 2;
                                    }
                                    i += j-1;
                                    curBlockLen = 0;
                                    clueCounter++;
                                }
                            }
                        }

                        clueCounter = clues.Length - 1;
                        lastGapPos = colLength - 1;
                        blockStartPos = colLength - 1;
                        curBlockLen = 0;

                        //backwards loop
                        for (int i = colLength - 1; i >= 0 && clueCounter >= 0; --i)
                        {
                            if (colWise[i][ci] == 2)
                            {
                                if (curBlockLen == 0) { blockStartPos = i; }
                                if (lastGapPos == i + 1)
                                {
                                    //fill in the rest of the clue if this is following off a known gap
                                    int j = 0;
                                    for (; j < clues[clueCounter] && blockStartPos - j >= 0 && colWise[blockStartPos - j][ci] != 1; ++j)
                                    {
                                        colWise[blockStartPos - j][ci] = 2;
                                    }
                                    j++;
                                    //if (blockStartPos - j >= 0) { colWise[blockStartPos - j][ci] = 1; }
                                    i -= j+1;
                                    curBlockLen = 0;
                                    clueCounter--;
                                    continue;
                                }
                                curBlockLen++;
                            }
                            else if (colWise[i][ci] == 1)
                            {
                                lastGapPos = i;
                                //check if we've completed the clue
                                if (curBlockLen > 0)// && curBlockLen == clues[clueCounter])
                                {
                                    clueCounter--;
                                    curBlockLen = 0;
                                }
                            }
                            else if (colWise[i][ci] == 0 && curBlockLen > 0 && clueCounter >= 0)
                            {
                                //derived from the mathematical approach used in Crowding
                                int excess = clues[clueCounter] - (colLength - lastGapPos) - (colLength - blockStartPos) - curBlockLen;
                                if (excess > 0)
                                {
                                    int j = 0;
                                    for (; j < excess && blockStartPos - j >= 0 && colWise[blockStartPos - j][ci] != 1; ++j)
                                    {
                                        colWise[blockStartPos - j][ci] = 2;
                                    }
                                    i -= j+1;
                                    curBlockLen = 0;
                                    clueCounter--;
                                }
                            }
                        }
                    }
                }
            );
                     
            return Update(ref puzzle, rowWise, colWise);
        }

        private PuzzleStatus Joining(ref byte[][] puzzle)
        {
            //opposite of Splitting
            throw new NotImplementedException();
        }


        /// <summary>
        /// A quick validity sum-check with the added bonus that it will note if the puzzle is solved
        /// </summary>
        /// <returns>
        /// PuzzleStatus reflective of this
        /// </returns>
        private PuzzleStatus ValidatePuzzle(byte[][] puzzle)
        {
            bool solved = true; //this'll change to false the moment a row or col doesn't have a sum match
            bool badRow = false, badCol = false;

            Parallel.Invoke(
                () =>
                {
                    for (int i = 0; !badRow && i < rowClues.Length; ++i)
                    {
                        int rowSum = rowClues[i].Split(' ').Select(n => int.Parse(n)).Sum();
                        int curSum = puzzle[i].Where(n => n > 0).Select(n => n - 1).Sum();
                        //int b = puzzle[i].Where(n => n == 1).Select(n => 1).Sum();

                        if (curSum > rowSum)
                        {
                            badRow = true;
                        }
                        else if (curSum < rowSum) { solved = false; } //this is a race only technically
                        //else if (!rowClues[i].Equals("0") && puzzle[i].Where(n => n == 1).Select(n => 1).Sum() == rowLength)
                        //{
                        //    badRow = true;
                        //}
                        else
                        {
                            //check that row has the right amount of blocks
                            int blockCtr = 0;
                            int blockLen = 0;
                            for (int j = 0; j < rowLength; ++j)
                            {
                                if (puzzle[i][j] == 2)
                                {
                                    if (blockLen == 0) { blockCtr++; }
                                    blockLen++;
                                }
                                else { blockLen = 0; }
                            }
                            if (rowClues[i].Equals("0") && blockCtr == 0) { continue; }
                            if (blockCtr != rowClues[i].Split(' ').Length) { badRow = true; }
                        }

                    }
                },
                () =>
                {
                    for (int i = 0; !badCol && i < colClues.Length; ++i)
                    {
                        int colSum = colClues[i].Split(' ').Select(n => int.Parse(n)).Sum();
                        //can't do this as LINQ, so loop
                        int curSum = 0;
                        for (int r = 0; r < rowClues.Length; r++)
                        {
                            if (puzzle[r][i] == 2) { curSum++; }
                        }

                        //int curSum = puzzle[i].Where(n => n > 0).Select(n => n - 1).Sum();

                        //if (curSum > colSum) { badCol = true; }
                        //if (curSum < colSum) { solved = false; } //this isn't a race b/c the only 

                        if (curSum > colSum)
                        {
                            badCol = true;
                        }
                        else if (curSum < colSum) { solved = false; } //this is a race only technically
                        //else if (!colClues[i].Equals("0") && puzzle[i].Where(n => n == 1).Select(n => 1).Sum() == rowLength)
                        //{
                        //    badCol = true;
                        //}
                        else
                        {
                            //check that row has the right amount of blocks
                            int blockCtr = 0;
                            int blockLen = 0;
                            for (int j = 0; j < colLength; ++j)
                            {
                                if (puzzle[j][i] == 2)
                                {
                                    if (blockLen == 0) { blockCtr++; }
                                    blockLen++;
                                }
                                else { blockLen = 0; }
                            }
                            if (colClues[i].Equals("0") && blockCtr == 0) { continue; }
                            if (blockCtr != colClues[i].Split(' ').Length) { badCol = true; }
                        }

                    }

                }
                );
            if (badRow || badCol) { return PuzzleStatus.Conflict; }

            //if there's no conflict, check if there were any changes, th

            //if the row sums match but the solution doesn't, then this is a bad solution, so return Conflict
            if (solved)
            {
                return ConfirmFullSolution(puzzle) ? PuzzleStatus.Solved : PuzzleStatus.Conflict;
            }

            return PuzzleStatus.Partial;

        }

        //return false if no changes, true otherwise
        private bool ApplyChanges(ref byte[][] original, byte[][] changes)
        {
            bool changesHappened = false;
            for(int r = 0; r < original.Length; ++r)
            {
                for(int c = 0; c <original[r].Length; ++c)
                {
                    if (original[r][c] != changes[r][c])
                    {
                        changesHappened = true;
                        goto PostLoop;
                    }
                }
            }
            PostLoop:
            if (changesHappened) { original = changes; }

            //if (original.SequenceEqual(changes)) return false;
            //apply now
            
            return changesHappened;
        }

        // This part looks at the "solved" puzzle, and validates that everything is an exact match
        private bool ConfirmFullSolution(byte[][] puzzle)
        {
            bool solved = true;
            Parallel.Invoke(
                () =>
                {
                    //iterate through the rows
                    for (int ri = 0; ri < rowClues.Count(); ri++)
                    {
                        var clues = rowClues[ri].Split(' ');
                        int clueCounter = 0;
                        int blockLenCtr = 0;
                        //loop through this specific row
                        for (int i = 0; solved && i < rowLength; i++)
                        {
                            if (puzzle[ri][i] == 2)
                                blockLenCtr++;
                            else
                            {
                                if (blockLenCtr > 0)
                                {
                                    if (clueCounter < clues.Length && blockLenCtr == int.Parse(clues[clueCounter]))
                                    {
                                        blockLenCtr = 0;
                                        clueCounter++;
                                    }
                                    else
                                    {
                                        solved = false;
                                    }
                                }
                            }
                        }
                    }
                },
                () =>
                {
                    //iterate through the cols
                    for (int ci = 0; ci < colClues.Count(); ci++)
                    {
                        var clues = colClues[ci].Split(' ');
                        int clueCounter = 0;
                        int blockLenCtr = 0;
                        //loop through this specific column
                        for (int i = 0; solved && i < rowClues.Length; i++)
                        {
                            if (puzzle[i][ci] == 2)
                                blockLenCtr++;
                            else
                            {
                                if (blockLenCtr > 0)
                                {
                                    if (clueCounter < clues.Length && blockLenCtr == int.Parse(clues[clueCounter]))
                                    {
                                        blockLenCtr = 0;
                                        clueCounter++;
                                    }
                                    else
                                    {
                                        solved = false;
                                    }
                                }
                            }
                        }
                    }
                }
             );
            return solved;
        }

        //ref is needed becuase we overwrite the pointer if the merge suceeds
        private bool Merge(byte[][] rowWise, byte[][] colWise, ref byte[][] puzzle)
        {
            //merging
            //put everything onto a temp puzzle, then copy it onto the official copy if it passes

            byte[][] temp = CopyArray(puzzle);
  
            for (int outer = 0; outer < temp.Length; outer++)
            {
                for (int inner = 0; inner < temp[outer].Length; inner++)
                {
                    if ((rowWise[outer][inner] == 1 && colWise[outer][inner] == 2) ||
                        (rowWise[outer][inner] == 2 && colWise[outer][inner] == 1))
                    {
                        return false;
                    }
                    //we now know that if they have value, it's either the same or on one dimension it's known and the other it isn't
                    if (rowWise[outer][inner] != 0) { temp[outer][inner] = rowWise[outer][inner]; }
                    else if (colWise[outer][inner] != 0) { temp[outer][inner] = colWise[outer][inner]; }
                }
            }

            //copy back since there was no conflict
            puzzle = temp;

            return true;
        }

        private PuzzleStatus Update(ref byte[][] puzzle, byte[][] rowWise, byte[][] colWise)
        {
            //update the called puzzle
            byte[][] newSolution = CopyArray(puzzle);
            if (Merge(rowWise, colWise, ref newSolution))
            {
                PuzzleStatus status = ValidatePuzzle(newSolution);
                if (status != PuzzleStatus.Conflict)
                {
                    if (ApplyChanges(ref puzzle, newSolution) || status == PuzzleStatus.Solved)
                    {
                        return status;
                    }
                    else { return PuzzleStatus.NoChange; }
                }
            }
            return PuzzleStatus.Conflict;
        }

        //Convert the byte[][] Puzzle to a bool[,] form
        public bool[,] GetSolvedPuzzle()
        {
            bool[,] returnPuzzle = new bool[puzzle.Length, puzzle[0].Length];
            for(int r = 0; r < puzzle.Length; r++)
            {
                for(int c = 0; c < puzzle[r].Length; c++)
                {
                    returnPuzzle[r, c] = (puzzle[r][c] == 2);
                }
            }

            return returnPuzzle;
        }
    }
}
