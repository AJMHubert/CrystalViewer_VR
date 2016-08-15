using System;
using System.Collections.Generic;

//
// Created by Jon on 23/08/2015.
//

//
// Created by Jon on 23/08/2015.
//




//class Symmetry;

public class SymmetryOperation
{
//C++ TO C# CONVERTER TODO TASK: C# has no concept of a 'friend' class:
//	friend class Symmetry;
	protected SymmetryOperation()
	{
	}
	public double xf;
	public double yf;
	public double zf;
	public double c;

	public SymmetryOperation(List<string> factors)
	{
		xf = 0;
		yf = 0;
		zf = 0;
		c = 0;

		foreach (string term in factors)
		{
			if (term.back() == 'x')
			{
				term.pop_back();
				xf += fractionToDecimal(term, 1.0);
			}
			else if (term.back() == 'y')
			{
				term.pop_back();
				yf += fractionToDecimal(term, 1.0);
			}
			else if (term.back() == 'z')
			{
				term.pop_back();
				zf += fractionToDecimal(term, 1.0);
			}
			else
			{
				c += fractionToDecimal(term, 0.0);
			}
		}
	}

	public double applyOperation(double xin, double yin, double zin)
	{
		return xf * xin + yf * yin + zf * zin + c;
	}

	public double applyOperation(Eigen.Vector3d positions)
	{
		return applyOperation(positions(0), positions(1), positions(2));
	}
	private double fractionToDecimal(string fractionstring, double nullReturn)
	{
		if (fractionstring == "")
		{
			return nullReturn;
		}

		if (fractionstring == "+")
		{
			return 1.0;
		}
		else if (fractionstring == "-")
		{
			return -1.0;
		}

		List<string> fsplit = GlobalMembers.Utilities.split(fractionstring, '/');

		return Convert.ToDouble(fsplit[0]) / Convert.ToDouble(fsplit[1]);
	}

}

// Small class just to hold the symmetry operation for all directions
public class Symmetry
{

	public Symmetry()
	{
	}

//    void setXOperation(std::vector<std::string> factors);
//    void setYOperation(std::vector<std::string> factors);
//    void setZOperation(std::vector<std::string> factors);
	public SymmetryOperation getOperation(int i)
	{
		if (i == 0)
		{
			return xOperation;
		}
		else if (i == 1)
		{
			return yOperation;
		}
		else if (i == 2)
		{
			return zOperation;
		}
	}

	public void setOperation(int i, List<string> factors)
	{
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: SymmetryOperation temp = SymmetryOperation(factors);
		SymmetryOperation temp = new SymmetryOperation(new List(factors));

		if (i == 0)
		{
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: xOperation = temp;
			xOperation.CopyFrom(temp);
		}
		else if (i == 1)
		{
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: yOperation = temp;
			yOperation.CopyFrom(temp);
		}
		else if (i == 2)
		{
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: zOperation = temp;
			zOperation.CopyFrom(temp);
		}
		// TODO: else statement with error handling
	}

	private SymmetryOperation xOperation = new SymmetryOperation();
	private SymmetryOperation yOperation = new SymmetryOperation();
	private SymmetryOperation zOperation = new SymmetryOperation();
}
