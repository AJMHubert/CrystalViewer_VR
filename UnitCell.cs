using System.Collections.Generic;

//
// Created by Jon on 21/08/2015.
//




public class UnitCell
{
	// constructor that accepts lengths and angles of unit cell
	public UnitCell(CellGeometry geo, List<AtomSite> sites)
	{
		this.geometry = new CellGeometry(geo);
		this.atoms = new List<AtomSite>(sites);
	}

	public CellGeometry getCellGeometry()
	{
		return geometry;
	}
	public List<AtomSite> getAtoms()
	{
		return atoms;
	}

	private CellGeometry geometry = new CellGeometry();

	private List<AtomSite> atoms = new List<AtomSite>();
//    CellGeometry geometry;
//
//    std::vector<AtomSite> atoms;

	// method to convert lengths and angles to 3 vectors
}


