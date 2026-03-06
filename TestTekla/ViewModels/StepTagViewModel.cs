using System;
using Tekla.Structures.Drawing;
using Tekla.Structures.Drawing.UI;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;

namespace TeklaApp.ViewModels
{
    public class StepTagViewModel
    {
        public string CreateStepTag()
        {
            DrawingHandler dh = new DrawingHandler();
            if (dh.GetActiveDrawing() == null)
            {
                return "Lỗi: Vui lòng mở một bản vẽ (Drawing) trước khi chạy lệnh này.";
            }

            try
            {
                Picker picker = dh.GetPicker();
                DrawingObject dPart1 = null;
                DrawingObject dPart2 = null;

                try
                {
                    var pick1 = picker.PickObject("Chọn sàn đầu tiên (sàn 1)");
                    dPart1 = pick1.Item1;

                    var pick2 = picker.PickObject("Chọn sàn thứ hai (sàn 2)");
                    dPart2 = pick2.Item1;
                }
                catch
                {
                    return "Đã hủy chọn sàn.";
                }

                Tekla.Structures.Drawing.Part dp1 = dPart1 as Tekla.Structures.Drawing.Part;
                Tekla.Structures.Drawing.Part dp2 = dPart2 as Tekla.Structures.Drawing.Part;

                if (dp1 == null || dp2 == null)
                {
                    return "Đối tượng được chọn không phải là Cấu kiện (Part) trong bản vẽ.";
                }

                Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
                Tekla.Structures.Model.Part mPart1 = model.SelectModelObject(dp1.ModelIdentifier) as Tekla.Structures.Model.Part;
                Tekla.Structures.Model.Part mPart2 = model.SelectModelObject(dp2.ModelIdentifier) as Tekla.Structures.Model.Part;

                if (mPart1 == null || mPart2 == null)
                {
                    return "Không tìm thấy cấu kiện tương ứng trong mô hình 3D.";
                }

                Solid solid1 = mPart1.GetSolid();
                Solid solid2 = mPart2.GetSolid();

                double z1 = solid1.MaximumPoint.Z;
                double z2 = solid2.MaximumPoint.Z;

                if (Math.Abs(z1 - z2) < 0.1)
                {
                    return "Hai sàn có cùng cao độ (không có giật cấp).";
                }

                int stepValue = (int)Math.Round(Math.Abs(z1 - z2));
                bool isPart1High = z1 > z2;

                Tekla.Structures.Geometry3d.Point pStart;
                Tekla.Structures.Geometry3d.Point pEnd;
                try
                {
                    var ptPick1 = picker.PickPoint("Chọn điểm đặt ký hiệu (trên mép giao của 2 sàn)");
                    pStart = ptPick1.Item1;
                    var ptPick2 = picker.PickPoint("Chọn hướng dọc theo mép giao (điểm thứ 2)");
                    pEnd = ptPick2.Item1;
                }
                catch
                {
                    return "Đã hủy chọn điểm.";
                }

                ViewBase view = dp1.GetView();
                double scale = 1.0;
                Tekla.Structures.Geometry3d.Point center1_view = new Tekla.Structures.Geometry3d.Point(0, 0, 0);
                if (view is Tekla.Structures.Drawing.View realView)
                {
                    scale = realView.Attributes.Scale;

                    // Transform center 1 to view to know the relative direction of the high part
                    Tekla.Structures.Geometry3d.Point center1_world = new Tekla.Structures.Geometry3d.Point(
                        (solid1.MinimumPoint.X + solid1.MaximumPoint.X) / 2.0,
                        (solid1.MinimumPoint.Y + solid1.MaximumPoint.Y) / 2.0,
                        (solid1.MinimumPoint.Z + solid1.MaximumPoint.Z) / 2.0
                    );

                    Tekla.Structures.Geometry3d.CoordinateSystem sys = realView.DisplayCoordinateSystem;
                    Matrix matrix = MatrixFactory.ToCoordinateSystem(sys);
                    center1_view = matrix.Transform(center1_world);
                }

                Vector vY = new Vector(pEnd.X - pStart.X, pEnd.Y - pStart.Y, 0);
                vY.Normalize();
                Vector vX = new Vector(vY.Y, -vY.X, 0); // Perpendicular

                Vector vecToCenter = new Vector(center1_view.X - pStart.X, center1_view.Y - pStart.Y, 0);
                double dot = vecToCenter.Dot(vX);

                Vector vHighX, vLowX;
                if (isPart1High)
                {
                    vHighX = dot > 0 ? vX : new Vector(vX.X * -1, vX.Y * -1, vX.Z * -1);
                }
                else
                {
                    vHighX = dot > 0 ? new Vector(vX.X * -1, vX.Y * -1, vX.Z * -1) : vX;
                }
                vLowX = new Vector(vHighX.X * -1, vHighX.Y * -1, vHighX.Z * -1);

                double L = 3.0 * scale; // Adjust size by view scale

                // Polygon High
                PointList ptsHigh = new PointList();
                ptsHigh.Add(pStart);
                ptsHigh.Add(new Tekla.Structures.Geometry3d.Point(pStart.X + vHighX.X * L, pStart.Y + vHighX.Y * L));
                ptsHigh.Add(new Tekla.Structures.Geometry3d.Point(pStart.X + vHighX.X * L + vY.X * L, pStart.Y + vHighX.Y * L + vY.Y * L));
                ptsHigh.Add(new Tekla.Structures.Geometry3d.Point(pStart.X + vY.X * L, pStart.Y + vY.Y * L));

                Tekla.Structures.Drawing.Polygon polyHigh = new Tekla.Structures.Drawing.Polygon(view, ptsHigh);
                polyHigh.Attributes = new Tekla.Structures.Drawing.Polygon.PolygonAttributes();
                polyHigh.Attributes.Hatch.Name = "ANSI31";
                polyHigh.Attributes.Hatch.Color = Tekla.Structures.Drawing.DrawingHatchColors.Green;
                polyHigh.Attributes.Line.Color = Tekla.Structures.Drawing.DrawingColors.Green; // Use green border to blend
                polyHigh.Insert();

                // Polygon Low
                PointList ptsLow = new PointList();
                ptsLow.Add(pStart);
                ptsLow.Add(new Tekla.Structures.Geometry3d.Point(pStart.X + vLowX.X * L, pStart.Y + vLowX.Y * L));
                ptsLow.Add(new Tekla.Structures.Geometry3d.Point(pStart.X + vLowX.X * L - vY.X * L, pStart.Y + vLowX.Y * L - vY.Y * L));
                ptsLow.Add(new Tekla.Structures.Geometry3d.Point(pStart.X - vY.X * L, pStart.Y - vY.Y * L));

                Tekla.Structures.Drawing.Polygon polyLow = new Tekla.Structures.Drawing.Polygon(view, ptsLow);
                polyLow.Attributes = new Tekla.Structures.Drawing.Polygon.PolygonAttributes();
                polyLow.Attributes.Hatch.Name = "ANSI31";
                polyLow.Attributes.Hatch.Color = Tekla.Structures.Drawing.DrawingHatchColors.Green;
                polyLow.Attributes.Line.Color = Tekla.Structures.Drawing.DrawingColors.Green; // Use green border to blend
                polyLow.Insert();

                // Z lines connecting them
                Tekla.Structures.Geometry3d.Point pTopHigh = new Tekla.Structures.Geometry3d.Point(pStart.X + vHighX.X * L + vY.X * L, pStart.Y + vHighX.Y * L + vY.Y * L);
                Tekla.Structures.Geometry3d.Point pMidHigh = new Tekla.Structures.Geometry3d.Point(pStart.X + vY.X * L, pStart.Y + vY.Y * L);
                Tekla.Structures.Geometry3d.Point pMidLow = new Tekla.Structures.Geometry3d.Point(pStart.X - vY.X * L, pStart.Y - vY.Y * L);
                Tekla.Structures.Geometry3d.Point pBottomLow = new Tekla.Structures.Geometry3d.Point(pStart.X + vLowX.X * L - vY.X * L, pStart.Y + vLowX.Y * L - vY.Y * L);

                Tekla.Structures.Drawing.Line line1 = new Tekla.Structures.Drawing.Line(view, pTopHigh, pMidHigh);
                line1.Insert();

                Tekla.Structures.Drawing.Line line2 = new Tekla.Structures.Drawing.Line(view, pMidHigh, pMidLow);
                line2.Insert();

                Tekla.Structures.Drawing.Line line3 = new Tekla.Structures.Drawing.Line(view, pMidLow, pBottomLow);
                line3.Insert();

                // Elevation text
                Tekla.Structures.Geometry3d.Point textPos = new Tekla.Structures.Geometry3d.Point(
                    pStart.X + vHighX.X * (L * 1.5) + vY.X * (L / 2),
                    pStart.Y + vHighX.Y * (L * 1.5) + vY.Y * (L / 2)
                );
                Text text = new Text(view, textPos, stepValue.ToString());
                text.Attributes = new Text.TextAttributes();
                text.Attributes.Font.Height = 3.5;
                text.Attributes.Font.Color = Tekla.Structures.Drawing.DrawingColors.Green;
                // Text angle is in DEGREES in TextAttributes
                double angleDeg = Math.Atan2(vY.Y, vY.X) * 180.0 / Math.PI;
                if (angleDeg > 90 || angleDeg <= -90)
                {
                    angleDeg += 180;
                }
                text.Attributes.Angle = angleDeg;
                text.Insert();

                dh.GetActiveDrawing().CommitChanges();
                return $"Thành công: Đã tạo ký hiệu giật cấp {stepValue}mm";
            }
            catch (Exception ex)
            {
                return "Đã xảy ra lỗi: " + ex.Message;
            }
        }
    }
}
