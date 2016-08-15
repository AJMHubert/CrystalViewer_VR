using System.Collections.Generic;

//
// Created by Jon on 24/08/2015.
//

//
// Created by Jon on 24/08/2015.
//




public class AtomSite
{
	public AtomSite()
	{
	}
	//AtomSite(std::string namein, double xin, double yin, double zin, double occin = 1.0);

	public AtomSite(List<Symmetry> symmetryvector, string namein, double xin, double yin, double zin, double occin = 1.0)
	{
		name.Add(namein);
		positions.Add(Eigen.Vector3d(wrapPosition(xin), wrapPosition(yin), wrapPosition(zin)));
		occupancy.Add(occin);

//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: applySymmetry(symmetryvector);
		applySymmetry(new List(symmetryvector));
	}

	public void applySymmetry(Symmetry symmetry)
	{
		double a = wrapPosition(symmetry.getOperation(0).applyOperation(positions[0]));
		double b = wrapPosition(symmetry.getOperation(1).applyOperation(positions[0]));
		double c = wrapPosition(symmetry.getOperation(2).applyOperation(positions[0]));

		var ctemp = Eigen.Vector3d(a, b, c);
		if (GlobalMembers.Utilities.vectorSearch(positions, ctemp) >= positions.Count)
		{
			positions.Add(ctemp);
		}
	}

	public void applySymmetry(List<Symmetry> symmetryvector)
	{
		foreach (Symmetry s in symmetryvector)
		{
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: applySymmetry(s);
			applySymmetry(new Symmetry(s));
		}
	}

	public void addAtom(string namein, double occin)
	{
		name.Add(namein);
		occupancy.Add(occin);
	}

	public List<double> getOccupancies()
	{
		return occupancy;
	}
	public List<Eigen.Vector3d> getPositions()
	{
		return positions;
	}
	public List<string> getElements()
	{
		return name;
	}

	private List<double> occupancy = new List<double>();
	private List<Eigen.Vector3d> positions = new List<Eigen.Vector3d>();
	private List<string> name = new List<string>();

	private double wrapPosition(double pos)
	{
		double dud;
		if (pos >= 1.0)
		{
			pos = std.modf(pos, dud);
		}
		else if (pos < 0.0)
		{
			pos = 1 + std.modf(pos, dud);
		}
		return pos;
	}

}
