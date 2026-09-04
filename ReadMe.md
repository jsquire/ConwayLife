# Conway's Game of Life Challenge

### Problem statement

Conway's Game of Life is a cellular automaton.  It operates on a two-dimensional grid where each cell can be either alive or dead.  Every generation, the grid ticks forward, and each cell's state changes according to a set of rules based on its eight surrounding neighbors.

The rules are simple.  An alive cell with fewer than two or more than three alive neighbors becomes dead.  A dead cell with exactly three alive neighbors becomes alive.  In all other cases, a cell keeps its current state.

The challenge is to implement this simulation in 64-bit signed integer space.  The coordinates of the live cells can be anywhere in the signed 64-bit range.

### Expected input

- A list of integer coordinates for live cells in the [Life 1.06 format](https://conwaylife.com/wiki/Life_1.06).  The first line is `#Life 1.06`, and each subsequent line is a pair of space-separated integers, the x and y coordinates of a live cell.  The coordinates can be anywhere in the signed 64-bit range.

- For example:

  ```text
  #Life 1.06
  0 1
  1 2
  2 0
  2 1
  2 2
  -2000000000000 -2000000000000
  -2000000000001 -2000000000001
  -2000000000000 -2000000000001
  ```

### Expected output

- The state of the simulation after a specified number of generations, in the Life 1.06 format.  The first line is `#Life 1.06`, and each subsequent line is a pair of space-separated integers, the x and y coordinates of a live cell.

### Constraints and assumptions

- The input is read from standard input.

- The output is written to standard output.

- The coordinates are signed 64-bit integers.

- The number of generations is parameterized.

- The eight surrounding cells are the neighbors.

- The representation of the input and output is the [Life 1.06 format](https://conwaylife.com/wiki/Life_1.06).

### Solution discussion

  Discussion of possible solutions and implementations can be found [here](./Solution.md).
