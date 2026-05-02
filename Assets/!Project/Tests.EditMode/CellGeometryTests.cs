using System.Collections.Generic;
using Game.Utils;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class CellGeometryTests
    {
        static List<Vector2Int> Cells(params (int x, int y)[] coords)
        {
            var list = new List<Vector2Int>(coords.Length);
            foreach (var (x, y) in coords) list.Add(new Vector2Int(x, y));
            return list;
        }
        
        //GET INTERSECTION
        
        [Test]
        public void GetIntersection_TShape_ReturnsPivot()
        {
            var cells = Cells((0, 2), (1, 2), (2, 2), (1, 1), (1, 0));
            var result = CellGeometry.GetIntersection(cells);
            Assert.AreEqual(new Vector2Int(1, 2), result);
        }

        [Test]
        public void GetIntersection_PlusShape_ReturnsCenter()
        {
            var cells = Cells((1, 0), (0, 1), (1, 1), (2, 1), (1, 2));
            var result = CellGeometry.GetIntersection(cells);
            Assert.AreEqual(new Vector2Int(1, 1), result);
        }

        [Test]
        public void GetIntersection_LShape_ReturnsCorner()
        {
            var cells = Cells((0, 0), (1, 0), (2, 0), (0, 1), (0, 2));
            var result = CellGeometry.GetIntersection(cells);
            Assert.AreEqual(new Vector2Int(0, 0), result);
        }

        [Test]
        public void GetIntersection_StraightLine_ReturnsAnyInternalCell()
        {
            var cells = Cells((0, 0), (1, 0), (2, 0), (3, 0));
            var result = CellGeometry.GetIntersection(cells);
            Assert.AreEqual(new Vector2Int(1, 0), result);
        }

        [Test]
        public void GetIntersection_SingleCell_ReturnsItself()
        {
            var cells = Cells((5, 7));
            var result = CellGeometry.GetIntersection(cells);
            Assert.AreEqual(new Vector2Int(5, 7), result);
        }

        [Test]
        public void GetBottomLeft_Square_ReturnsMinCorner()
        {
            var cells = Cells((2, 3), (3, 3), (2, 4), (3, 4));
            var result = CellGeometry.GetBottomLeft(cells);
            Assert.AreEqual(new Vector2Int(2, 3), result);
        }

        [Test]
        public void GetBottomLeft_ScatteredCells_ReturnsCombinedMin()
        {
            var cells = Cells((5, 2), (1, 7), (3, 4));
            var result = CellGeometry.GetBottomLeft(cells);
            Assert.AreEqual(new Vector2Int(1, 2), result);
        }

        [Test]
        public void GetBottomLeft_NegativeCoordinates_Works()
        {
            var cells = Cells((-1, -2), (0, 0), (1, 2));
            var result = CellGeometry.GetBottomLeft(cells);
            Assert.AreEqual(new Vector2Int(-1, -2), result);
        }

        //GET CENTER
        
        [Test]
        public void GetCenter_OddCount_ReturnsMiddleByIndex()
        {
            var cells = Cells((0, 0), (1, 0), (2, 0));
            var result = CellGeometry.GetCenter(cells);
            Assert.AreEqual(new Vector2Int(1, 0), result);
        }

        [Test]
        public void GetCenter_EvenCount_ReturnsUpperMiddleByIndex()
        {
            var cells = Cells((0, 0), (1, 0), (2, 0), (3, 0));
            var result = CellGeometry.GetCenter(cells);
            Assert.AreEqual(new Vector2Int(2, 0), result);
        }

        [Test]
        public void GetCenter_SingleCell_ReturnsItself()
        {
            var cells = Cells((4, 5));
            var result = CellGeometry.GetCenter(cells);
            Assert.AreEqual(new Vector2Int(4, 5), result);
        }
    }
}