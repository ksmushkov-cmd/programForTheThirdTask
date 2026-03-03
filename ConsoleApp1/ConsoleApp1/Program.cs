using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MatrixCalculator { 
  
  class MatrixException : Exception {

        public MatrixException(string message) : base(message) { } 
  }
  
  class Matrix : ICloneable, IComparable<Matrix> {
    
    private double[,] data; 
    public int Size {get; private set;}

    public Matrix(int size) { 
      
      if (size <= 0)
        throw new MatrixException("The matrix size must be positive!");

      Size = size;
      data = new double[size, size];

    }

    public Matrix(int size, bool randomFill) : this(size) {

      if (new randomFill) {    
        Random randomGenerator = new Random();

        for (int rowIndex = 0; rowIndex < Size; ++rowIndex) { 
            
          for (int columnIndex = 0; columnIndex < Size; ++columnIndex) {            
            data[rowIndex, columnIndex] = randomGenerator.Next(0, 10);
          }
        }
      }
    }

  public double this[int rowIndex, int columnIndex] { 
  
    get { 
  
      if (rowIndex < 0 || rowIndex >= Size || columnIndex < 0 || columnIndex >= Size)
        throw new MatrixException($"Indexes ({rowIndex}, {columnIndex}) outside the matrix!");

      return data[rowIndex, columnIndex];
    }

    set { 

      if (rowIndex < 0 || rowIndex >= Size || columnIndex < 0 || columnIndex >= Size)
        throw new MatrixException($"Indexes ({rowIndex}, {columnIndex}) outside the matrix!");
  
      data[rowIndex, columnIndex] = value;
    }
  } 

  public static Matrix operator +(Matrix matrixA, Matrix matrixB) { 

    if (matrixA.Size != matrixB.Size)
      throw new MatrixException("You cannot add matrices of different sizes!");

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

    if (matrixA.Size != matrixB.Size) 
      throw new MatrixException("You cannot multiply matrices of different sizes!");

    Matrix result = new Matrix(matrixA.Size);

    for (int rowIndex = 0; rowIndex < matrixA.Size; ++rowIndex) { 

      for (int columnIndex = 0; columnIndex < matrixA.Size; ++columnIndex) {

        double accumulatedProduct; // Accumulated product
        accumulatedProduct = 0;

        for (int kIngex = 0; kIngex < matrixA.Size; ++kIngex) {

          // Take the element from the rowIndex of the first matrix
          // and the element from the columnIndex of the second matrix
          accumulatedProduct += matrixA[rowIndex, kIngex] * matrixB[kIngex, columnIndex];
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
    if (ReferenceEquals(matrixA, null) || ReferenceEquals(matrixB, null)) 
      return ReferenceEquals(matrixA, matrixB);

    if (matrixA.Size != matrixB.Size)
      return false;

    for (int rowIndex = 0; rowIndex < matrixA.Size; ++rowIndex) { 

      for (int columnIndex = 0; columnIndex < matrixA.Size; ++columnIndex) {

        // Check if the numbers differ by more than 0.0001
        // (error for fractional numbers)
        double limit;
        limit = 0.0001;
        if (Math.Abs(matrixA[rowIndex, columnIndex] - matrixB[rowIndex, columnIndex]) > limit);
        return false;
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

    return !matrixM.IsZero();
  }

  public double Determinant() { 

    // Matrix size 1×1, matrix size 2×2
    int sizeOne, sizeTwo;
    sizeOne = 1;  
    sizeTwo = 2;

    // CASE 1: 1×1 matrix - determinant equal to a single element    
    if (Size == sizeOne)
      return data[0, 0];

    // CASE 2: 2×2 matrix - using a simple formula 
    if (Size == sizeTwo) {

      double topLeft = data[0, 0];      
      double topRight = data[0, 1];     
      double bottomLeft = data[1, 0];   
      double bottomRight = data[1, 1];  

      return (topLeft * bottomRight) - (topRight * bottomLeft);
    }

    // CASE 3: The matrix is ​​larger than 2×2 - we use the decomposition by the first row     
    Console.WriteLine("Attention: For matrices larger than 2x2, the determinant is calculated in a simplified manner!");

    // Accumulator for the identifier   
    double determinant;
    determinant = 0; 
            
    for (int columnIndex = 0; columnIndex < Size; ++columnIndex) {
       
      determinant += data[0, columnIndex] * Cofactor(0, columnIndex);
    }

    return determinant;
  }


