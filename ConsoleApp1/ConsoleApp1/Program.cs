using System;
using System.Collections.Generic;

namespace MatrixCalculator { 
  
  class MatrixException : Exception {
     public MatrixException(string message) : base(message) { } 
  }
  
  class Matrix : ICloneable, IComparable<Matrix> {
    
    private double[,] _matrix; 
    public int Size {get; private set;}

    public Matrix(int size) { 
      
      if (size <= 0) { 
        throw new MatrixException("The matrix size must be positive!");
      }

      Size = size;
      _matrix = new double[size, size];

    }

    public Matrix(int size, bool randomFill) : this(size) {

      if (randomFill) {    
        Random randomGenerator = new Random();

        for (int rowIndex = 0; rowIndex < Size; ++rowIndex) { 
            
          for (int columnIndex = 0; columnIndex < Size; ++columnIndex) {            
            _matrix[rowIndex, columnIndex] = randomGenerator.Next(0, 10);
          }
        }
      }
    }

    public double this[int rowIndex, int columnIndex] { 
  
      get { 
  
        if (rowIndex < 0 || rowIndex >= Size || columnIndex < 0 || columnIndex >= Size) { 
          throw new MatrixException($"Indexes ({rowIndex}, {columnIndex}) outside the matrix!");
        }

        return _matrix[rowIndex, columnIndex];
      }

      set { 

        if (rowIndex < 0 || rowIndex >= Size || columnIndex < 0 || columnIndex >= Size) { 
          throw new MatrixException($"Indexes ({rowIndex}, {columnIndex}) outside the matrix!");
        }

        _matrix[rowIndex, columnIndex] = value;
      }
    } 

    public static Matrix operator +(Matrix matrixA, Matrix matrixB) { 

      if (matrixA.Size != matrixB.Size) { 
        throw new MatrixException("You cannot add matrices of different sizes!");
      }

      Matrix result = new Matrix(matrixA.Size);

      for (int rowIndex = 0; rowIndex < matrixA.Size; ++rowIndex) { 

        for (int columnIndex = 0; columnIndex < matrixA.Size; ++columnIndex) {
          result[rowIndex, columnIndex] = matrixA[rowIndex, columnIndex] +
                                          matrixB[rowIndex, columnIndex];
        }
      }

      return result;
    }

    public static Matrix operator *(Matrix matrixA, Matrix matrixB) { 

      if (matrixA.Size != matrixB.Size) { 
        throw new MatrixException("You cannot multiply matrices of different sizes!");
      }

      Matrix result = new Matrix(matrixA.Size);

      for (int rowIndex = 0; rowIndex < matrixA.Size; ++rowIndex) { 

        for (int columnIndex = 0; columnIndex < matrixA.Size; ++columnIndex) {

          double accumulatedProduct = 0; // Accumulated product 

          for (int kIndex = 0; kIndex < matrixA.Size; ++kIndex) {

            // Take the element from the rowIndex of the first matrix
            // and the element from the columnIndex of the second matrix
            accumulatedProduct += matrixA[rowIndex, kIndex] * matrixB[kIndex, columnIndex];
          }

          result[rowIndex, columnIndex] = accumulatedProduct;
        }
      }

      return result;
    }

    public static bool operator >(Matrix matrixA, Matrix matrixB) { 

      return matrixA.SumElements() > matrixB.SumElements();
    }

    public static bool operator <(Matrix matrixA, Matrix matrixB) { 

      return matrixA.SumElements() < matrixB.SumElements();
    }

    public static bool operator >=(Matrix matrixA, Matrix matrixB) { 

      return matrixA.SumElements() >= matrixB.SumElements();
    }

    public static bool operator <=(Matrix matrixA, Matrix matrixB) { 

      return matrixA.SumElements() <= matrixB.SumElements();
    }
     
    public static bool operator ==(Matrix matrixA, Matrix matrixB) {
  
      // Check if one of the matrices is empty (null)
      if (ReferenceEquals(matrixA, null) || ReferenceEquals(matrixB, null)) { 
        return ReferenceEquals(matrixA, matrixB);
      }

      if (matrixA.Size != matrixB.Size) { 
        return false;
      }

      for (int rowIndex = 0; rowIndex < matrixA.Size; ++rowIndex) { 

        for (int columnIndex = 0; columnIndex < matrixA.Size; ++columnIndex) {

          // Check if the numbers differ by more than 0.0001
          // (error for fractional numbers)
          double epsilon = 0.0001;

          if (Math.Abs(matrixA[rowIndex, columnIndex] - matrixB[rowIndex, columnIndex]) > epsilon) { 
            return false;
          }
        }
      }

      return true;
    }

    public static bool operator !=(Matrix matrixA, Matrix matrixB) { 

      return !(matrixA == matrixB);
    }

    // From number to matrix (create a 1x1 matrix)
    public static implicit operator Matrix(double number) { 
    
      Matrix resultMatrix = new Matrix(1);

      resultMatrix[0, 0] = number;
    
      return resultMatrix;
    }

    // From matrix to number (take the first element)
    public static explicit operator double(Matrix matrixM) {
  
      return matrixM[0, 0];
    }

    public static bool operator true(Matrix matrixM) { 

      return !matrixM.IsZero();
    }

    public static bool operator false(Matrix matrixM) { 

      return matrixM.IsZero();
    }

    public double Determinant() { 

      // Matrix size 1×1, matrix size 2×2
      int sizeOneByOne = 1;  
      int sizeTwoByTwo = 2;

      // CASE 1: 1×1 matrix - determinant equal to a single element    
      if (Size == sizeOneByOne) { 
        return _matrix[0, 0];
      }

      // CASE 2: 2×2 matrix - using a simple formula 
      if (Size == sizeTwoByTwo) {

        double topLeft = _matrix[0, 0];      
        double topRight = _matrix[0, 1];     
        double bottomLeft = _matrix[1, 0];   
        double bottomRight = _matrix[1, 1];  

        return (topLeft * bottomRight) - (topRight * bottomLeft);
      }

      // CASE 3: The matrix is ​​larger than 2×2 - we use the decomposition by the first row     
      Console.WriteLine("We calculate the determinant using the first row expansion method (recursion)");

      // Accumulator for the identifier   
      double determinant = 0; 
            
      for (int columnIndex = 0; columnIndex < Size; ++columnIndex) {
       
        determinant += _matrix[0, columnIndex] * Cofactor(0, columnIndex);
      }

      return determinant;
    }

    // Auxiliary method for the determinant     
    private double Cofactor(int row, int column) {

      Matrix minor = CreateMinor(row, column);
      return ((row + column) % 2 == 0 ? 1 : -1) * minor.Determinant();
    }

    private Matrix CreateMinor(int excludeRow, int excludeColumn) {

      int lessThanTheOriginal = 1;
      int minorSize;
      minorSize = Size - lessThanTheOriginal;  
      Matrix minor = new Matrix(minorSize);

      int minorRowIndex = 0;
      int minorColumnIndex = 0;
     
      for (int sourceRowIndex = 0; sourceRowIndex < Size; ++sourceRowIndex) {
              
        if (sourceRowIndex == excludeRow) { 
          continue;
        }
      
        // Start a new row in a minor - reset the column index
        minorColumnIndex = 0;

        for (int sourceColumnIndex = 0; sourceColumnIndex < Size; ++sourceColumnIndex) {
                    
          if (sourceColumnIndex == excludeColumn) { 
            continue;
          } 
          
          minor[minorRowIndex, minorColumnIndex] = _matrix[sourceRowIndex, sourceColumnIndex];       
          ++minorColumnIndex;
        }

        ++minorRowIndex;
      }

      return minor;
    }

    public Matrix Inverse() {

      int requiredSize = 2;

      if (Size != requiredSize) { 
        throw new MatrixException("The inverse matrix is ​​implemented only for 2x2 matrices!");
      }

      double determinant = Determinant();

      // Error for comparison with zero
      double epsilon = 0.0001;
           
      if (Math.Abs(determinant) < epsilon) { 
        throw new MatrixException("The matrix is ​​singular (the determinant is 0)!");
      }

      int resultSize = 2;
      Matrix result = new Matrix(resultSize);

      // Indices of the elements of the original matrix
      int topLeftRow, topLeftColumn, topRightRow, topRightColumn, bottomLeftRow, bottomLeftColumn, bottomRightRow, bottomRightColumn;
      topLeftRow = 0; // a - row 0, column 0
      topLeftColumn = 0;

      topRightRow = 0;    // b - row 0, column 1
      topRightColumn = 1;

      bottomLeftRow = 1;  // c - row 1, column 0
      bottomLeftColumn = 0;

      bottomRightRow = 1; // d - row 1, column 1
      bottomRightColumn = 1;

      // Fill in the inverse matrix using the formula
      result[0, 0] = _matrix[bottomRightRow, bottomRightColumn] / determinant;  
      result[0, 1] = -_matrix[topRightRow, topRightColumn] / determinant;      
      result[1, 0] = -_matrix[bottomLeftRow, bottomLeftColumn] / determinant;   
      result[1, 1] = _matrix[topLeftRow, topLeftColumn] / determinant;          

      return result;
    }
    private double SumElements() {
           
      double totalSum = 0;
   
      for (int rowIndex = 0; rowIndex < Size; ++rowIndex) {
                
        for (int columnIndex = 0; columnIndex < Size; ++columnIndex) {
                    
          totalSum += _matrix[rowIndex, columnIndex];
        }
      }

      return totalSum;
    }

    private bool IsZero() {
            
      for (int rowIndex = 0; rowIndex < Size; ++rowIndex) {
                
        for (int columnIndex = 0; columnIndex < Size; ++columnIndex) {
          // Check if the current element is non-zero
          // (with a 0.0001 tolerance for fractional numbers)
          double epsilon = 0.0001;
          if (Math.Abs(data[rowIndex, columnIndex]) > epsilon) {
                        
            return false;
          }
        }
      }

      return true;
    }

    public override string ToString() {
            
      string result = $"Matrix {Size}x{Size}:\n";
      
      for (int rowIndex = 0; rowIndex < Size; ++rowIndex) {
               
        result += "[ ";

        for (int columnIndex = 0; columnIndex < Size; ++columnIndex) {

          // "F2" means format with two decimal places
          result += _matrix[rowIndex, columnIndex].ToString("F2") + " ";
        }

        result += "]\n";
      }

      return result;
    }

    public override bool Equals(object objectA) {
            
      if (objectA is Matrix otherMatrix) { 
        return this == otherMatrix;  
      }

      return false;
    }

    public override int GetHashCode() {
           
      return _matrix.GetHashCode() ^ Size;
    }

    public int CompareTo(Matrix other) {

      if (other == null) { 
        return 1;
      }

      return this.SumElements().CompareTo(other.SumElements());
    }

    public object Clone() {
            
      Matrix clonedMatrix = new Matrix(this.Size);

      for (int rowIndex = 0; rowIndex < Size; ++rowIndex) {
                
        for (int columnIndex = 0; columnIndex < Size; ++columnIndex) {
                    
          clonedMatrix[rowIndex, columnIndex] = this[rowIndex, columnIndex];
        }
      }

      return clonedMatrix;
    }
  }

  class Program {
    static void Main(string[] args) {

      Console.WriteLine("WELCOME TO THE MATRIX CALCULATOR!\n" +
                        "=================================\n");

      try {
               
        Console.WriteLine("Let's create the first 3x3 matrix:");
        Matrix firstMatrix = new Matrix(3, true);
        Console.WriteLine(firstMatrix);

        Console.WriteLine("Create a second 3x3 matrix:");
        Matrix secondMatrix = new Matrix(3, true);
        Console.WriteLine(secondMatrix);

        Console.WriteLine("OPERATION DEMONSTRATION:\n" +
                          "------------------------");
        // Addition
        Console.WriteLine("firstMatrix + secondMatrix =");
        Matrix sumMatrix = firstMatrix + secondMatrix;
        Console.WriteLine(sumMatrix);

        // Multiplication
        Console.WriteLine("firstMatrix * secondMatrix =");
        Matrix productMatrix = firstMatrix * secondMatrix;
        Console.WriteLine(productMatrix);

        // Comparison
        Console.WriteLine($"firstMatrix > secondMatrix? {firstMatrix > secondMatrix}\n" +
                          $"firstMatrix < secondMatrix? {firstMatrix < secondMatrix}" +
                          $"firstMatrix == secondMatrix? {firstMatrix == secondMatrix}" +
                          $"firstMatrix != secondMatrix? {firstMatrix != secondMatrix}\n");

        // Determinant
        Console.WriteLine($"Determinant firstMatrix: {firstMatrix.Determinant():F2}\n" +
                          $"Determinant secondMatrix: {secondMatrix.Determinant():F2}\n");

        // Inverse matrix (for 2x2)
        Console.WriteLine("Let's create a 2x2 matrix for the inverse:");
        Matrix smallSquareMatrix = new Matrix(2, true);
        Console.WriteLine(smallSquareMatrix);
        Console.WriteLine($"Determinant smallSquareMatrix: {smallSquareMatrix.Determinant():F2}");

        // Check that the determinant is not zero (the matrix is ​​not singular)
        double epsilon = 0.0001;

        if (Math.Abs(smallSquareMatrix.Determinant()) > epsilon) {

          Console.WriteLine("Inverse matrix smallSquareMatrix:\n" + 
                            inverseMatrix + 
                            "\nExamination: smallSquareMatrix * inverseMatrix (must be single):\n" + 
                            smallSquareMatrix * inverseMatrix);
        } else {

          Console.WriteLine("The determinant is zero - the inverse matrix does not exist!");
        }

        // Type casting
        Console.WriteLine("Type casting:");
        double inputNumber = 5.5;
        Matrix matrixFromNumber = inputNumber;  // Implicit conversion of a number to a matrix
        Console.WriteLine($"Number {inputNumber} as a matrix:\n{matrixFromNumber}");

        Matrix randomMatrix = new Matrix(2, true);
        Console.WriteLine("Matrix 2x2:\n" + randomMatrix);
        double firstElement = (double)randomMatrix;  // eExplicit cast to number
        Console.WriteLine($"The first element as a number: {firstElement}\n");

        Matrix zeroMatrix = new Matrix(2);
        Console.WriteLine($"Zero matrix: {zeroMatrix}");
        if (zeroMatrix) { 
          Console.WriteLine("Zero matrix true");
        } else { 
          Console.WriteLine("Zero matrix false");
        }

        Console.WriteLine("\nPrototype (cloning):");
        Matrix originalMatrix = new Matrix(2, true);
        Matrix clonedMatrix = (Matrix)originalMatrix.Clone();
        Console.WriteLine("Original:\n" + 
                          originalMatrix + 
                          "\nCopy:\n" + 
                          clonedMatrix + 
                          $"\nOriginal == Copy? {originalMatrix == clonedMatrix}");

        clonedMatrix[0, 0] = 999;
        Console.WriteLine("After changing the copy:" + 
                          "Original[0,0] = {originalMatrix[0, 0]}" +
                          "Copy[0,0] = {clonedMatrix[0, 0]}" + 
                          "The original has not changed - deep copying works!");
      }

      catch (MatrixException matrixError) {
        Console.WriteLine($"Matrix error: {matrixError.Message}");
      }

      catch (Exception generalError) {
        Console.WriteLine($"Unexpected error: {generalError.Message}");
      }

      Console.WriteLine("\nThe program has terminated. Press any key...");
      Console.ReadKey();
    }
  }
}