using System;
using System.Collections.Generic;

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
      data = new double[sixe, size];

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



