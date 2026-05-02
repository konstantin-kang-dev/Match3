using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class MatchShapeRecognizerTests
    {
        //HELPERS

        static List<Vector2Int> Cells(params (int x, int y)[] coords)
        {
            var list = new List<Vector2Int>(coords.Length);
            foreach (var (x, y) in coords) list.Add(new Vector2Int(x, y));
            return list;
        }

        static List<Vector2Int> HorizontalLine(int startX, int y, int length)
        {
            var list = new List<Vector2Int>(length);
            for (int i = 0; i < length; i++) list.Add(new Vector2Int(startX + i, y));
            return list;
        }

        static List<Vector2Int> VerticalLine(int x, int startY, int length)
        {
            var list = new List<Vector2Int>(length);
            for (int i = 0; i < length; i++) list.Add(new Vector2Int(x, startY + i));
            return list;
        }

        //DEGENERATE CASES

        [Test]
        public void Recognize_EmptyList_ReturnsNull()
        {
            var result = MatchShapeRecognizer.Recognize(new List<Vector2Int>());
            Assert.IsNull(result);
        }

        [Test]
        public void Recognize_TwoCells_ReturnsNull()
        {
            var result = MatchShapeRecognizer.Recognize(Cells((0, 0), (1, 0)));
            Assert.IsNull(result);
        }

        [Test]
        public void Recognize_ThreeCellsNotInLine_ReturnsNull()
        {
            var result = MatchShapeRecognizer.Recognize(Cells((0, 0), (1, 0), (0, 1)));
            Assert.IsNull(result);
        }

        //MATCH

        [Test]
        public void Recognize_ThreeHorizontal_ReturnsMatch3()
        {
            var result = MatchShapeRecognizer.Recognize(HorizontalLine(0, 0, 3));
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match3, result.Value.Shape);
            Assert.AreEqual(3, result.Value.ShapeCells.Count);
        }

        [Test]
        public void Recognize_ThreeVertical_ReturnsMatch3()
        {
            var result = MatchShapeRecognizer.Recognize(VerticalLine(0, 0, 3));
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match3, result.Value.Shape);
        }

        //MATCH4LINE

        [Test]
        public void Recognize_FourHorizontal_ReturnsMatch4Horizontal()
        {
            var result = MatchShapeRecognizer.Recognize(HorizontalLine(0, 0, 4));
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match4Horizontal, result.Value.Shape);
            Assert.AreEqual(4, result.Value.ShapeCells.Count);
        }

        [Test]
        public void Recognize_FourVertical_ReturnsMatch4Vertical()
        {
            var result = MatchShapeRecognizer.Recognize(VerticalLine(0, 0, 4));
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match4Vertical, result.Value.Shape);
        }

        //MATCH4SQUARE

        [Test]
        public void Recognize_2x2Square_ReturnsMatch4Square()
        {
            var result = MatchShapeRecognizer.Recognize(Cells((0, 0), (1, 0), (0, 1), (1, 1)));
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match4Square, result.Value.Shape);
            Assert.AreEqual(4, result.Value.ShapeCells.Count);
        }

        [Test]
        public void Recognize_SquareWithTail_PrioritizesSquare()
        {
            var cells = Cells((0, 0), (1, 0), (0, 1), (1, 1), (2, 1));
            var result = MatchShapeRecognizer.Recognize(cells);
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match4Square, result.Value.Shape);
            Assert.AreEqual(4, result.Value.ShapeCells.Count);
        }

        //MATCH5LINE

        [Test]
        public void Recognize_FiveHorizontal_ReturnsMatch5Line()
        {
            var result = MatchShapeRecognizer.Recognize(HorizontalLine(0, 0, 5));
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match5Line, result.Value.Shape);
            Assert.AreEqual(5, result.Value.ShapeCells.Count);
        }

        [Test]
        public void Recognize_SevenInRow_ReturnsMatch5Line()
        {
            var result = MatchShapeRecognizer.Recognize(HorizontalLine(0, 0, 7));
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match5Line, result.Value.Shape);
            Assert.AreEqual(7, result.Value.ShapeCells.Count);
        }

        //MATCH5LT

        [Test]
        public void Recognize_TShape_ReturnsMatch5LT()
        {
            var cells = Cells((0, 2), (1, 2), (2, 2), (1, 1), (1, 0));
            var result = MatchShapeRecognizer.Recognize(cells);
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match5LT, result.Value.Shape);
            Assert.AreEqual(5, result.Value.ShapeCells.Count);
        }

        [Test]
        public void Recognize_LShape_ReturnsMatch5LT()
        {
            var cells = Cells((0, 0), (1, 0), (2, 0), (0, 1), (0, 2));
            var result = MatchShapeRecognizer.Recognize(cells);
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match5LT, result.Value.Shape);
        }

        [Test]
        public void Recognize_PlusShape_ReturnsMatch5LT()
        {
            var cells = Cells((1, 0), (0, 1), (1, 1), (2, 1), (1, 2));
            var result = MatchShapeRecognizer.Recognize(cells);
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match5LT, result.Value.Shape);
        }

        //PRIORITY

        [Test]
        public void Recognize_Line5AndLT_PrioritizesLine5()
        {
            var cells = Cells((0, 1), (1, 1), (2, 1), (3, 1), (4, 1), (2, 0));
            var result = MatchShapeRecognizer.Recognize(cells);
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match5Line, result.Value.Shape);
        }

        [Test]
        public void Recognize_LTAndLine4_PrioritizesLT()
        {
            var cells = Cells((0, 2), (1, 2), (2, 2), (3, 2), (0, 1), (0, 0));
            var result = MatchShapeRecognizer.Recognize(cells);
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match5LT, result.Value.Shape);
        }

        [Test]
        public void Recognize_SquareAndLine4_PrioritizesSquare()
        {
            var cells = Cells((0, 1), (1, 1), (2, 1), (3, 1), (0, 0), (1, 0));
            var result = MatchShapeRecognizer.Recognize(cells);
            Assert.IsNotNull(result);
            Assert.AreEqual(MatchShape.Match4Square, result.Value.Shape);
        }
    }
}