using System;

//
// Created by Jon on 25/08/2015.
//

//
// Created by Jon on 25/08/2015.
//




public class CellGeometry
{
	public CellGeometry()
	{
	}
	public CellGeometry(double ain, double bin, double cin, double alphain, double betain, double gammain)
	{
		a = ain;
		b = bin;
		c = cin;
		alpha = alphain;
		beta = betain;
		gamma = gammain;

		calculateCartesianBasis();
	}

	public Eigen.Vector3d getAVector()
	{
		return avec;
	}
	public Eigen.Vector3d getBVector()
	{
		return bvec;
	}
	public Eigen.Vector3d getCVector()
	{
		return cvec;
	}

	private double a;
	private double b;
	private double c;
	private double alpha;
	private double beta;
	private double gamma;

	private Eigen.Vector3d avec = new Eigen.Vector3d();
	private Eigen.Vector3d bvec = new Eigen.Vector3d();
	private Eigen.Vector3d cvec = new Eigen.Vector3d();

	private void calculateCartesianBasis()
	{
		// !! convert to radians
		double arad = alpha * (GlobalMembers.PI / 180);
		double brad = beta * (GlobalMembers.PI / 180);
		double grad = gamma * (GlobalMembers.PI / 180);

		// easy enough
		avec = Eigen.Vector3d(a, 0.0, 0.0);
		// slightly trickier
		bvec = Eigen.Vector3d(b * Math.Cos(grad), b * Math.Sin(grad), 0.0);
		// WOAH
		double cv1 = c * Math.Cos(brad);
		double cv2 = c * (Math.Cos(arad) - Math.Cos(brad) * Math.Cos(grad)) / Math.Sin(grad);
		double cv3 = c * Math.Sqrt((Math.Cos(brad - grad) - Math.Cos(arad)) * (Math.Cos(arad) - Math.Cos(brad + grad))) / Math.Sin(grad);

		cvec = Eigen.Vector3d(cv1, cv2, cv3);
	}
}
