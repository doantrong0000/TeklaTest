using System;
using Tekla.Structures.Model;
using TeklaApp.Models;

namespace TeklaApp.ViewModels
{
    public class MainViewModel
    {
        private TeklaModelMng _teklaModel;

        public MainViewModel()
        {
            _teklaModel = new TeklaModelMng();
        }

        public string ReadParameters()
        {
            if (!_teklaModel.IsConnected())
            {
                return "Lỗi: Không tìm thấy Tekla Structures đang mở.";
            }

            try
            {
                Tekla.Structures.Model.UI.Picker picker = new Tekla.Structures.Model.UI.Picker();
                ModelObject pickedObject = picker.PickObject(Tekla.Structures.Model.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "Vui lòng chọn đối tượng để đọc thông số");

                if (pickedObject is Part part)
                {
                    string info = "Thông số User Defined Attributes (UDA):\r\n\r\n";
                    info += $"- Tên cấu kiện: {part.Name}\r\n";

                    string userField1 = "";
                    part.GetUserProperty("USER_FIELD_1", ref userField1);
                    info += $"- User field 1: {userField1}\r\n";

                    string userField2 = "";
                    part.GetUserProperty("USER_FIELD_2", ref userField2);
                    info += $"- User field 2: {userField2}\r\n";

                    string userField3 = "";
                    part.GetUserProperty("USER_FIELD_3", ref userField3);
                    info += $"- User field 3: {userField3}\r\n";

                    string userField4 = "";
                    part.GetUserProperty("USER_FIELD_4", ref userField4);
                    info += $"- User field 4: {userField4}\r\n";

                    string comment = "";
                    part.GetUserProperty("comment", ref comment);
                    info += $"- Comment: {comment}\r\n";

                    string prelimMark = "";
                    part.GetUserProperty("PRELIM_MARK", ref prelimMark);
                    info += $"- Preliminary mark: {prelimMark}\r\n";

                    return info;
                }
                else
                {
                    return "Đối tượng được chọn không phải là Part. Loại: " + pickedObject.GetType().Name;
                }
            }
            catch (Exception ex)
            {
                return "Đã hủy chọn hoặc có lỗi: " + ex.Message;
            }
        }

        public string DeletePartCuts()
        {
            if (!_teklaModel.IsConnected())
            {
                return "Lỗi: Không tìm thấy Tekla Structures đang mở.";
            }

            try
            {
                Tekla.Structures.Model.UI.Picker picker = new Tekla.Structures.Model.UI.Picker();
                ModelObject pickedObject = picker.PickObject(Tekla.Structures.Model.UI.Picker.PickObjectEnum.PICK_ONE_PART, "Vui lòng chọn cấu kiện để xóa PartCut");

                if (pickedObject is Part hostPart)
                {
                    ModelObjectEnumerator cutEnumerator = hostPart.GetBooleans();
                    int cutCount = 0;

                    while (cutEnumerator.MoveNext())
                    {
                        if (cutEnumerator.Current is BooleanPart booleanCut)
                        {
                            cutCount++;
                            booleanCut.Delete();
                        }
                    }

                    if (cutCount > 0)
                    {
                        _teklaModel.Commit();
                        return $"Đã xóa {cutCount} PartCut thành công.";
                    }
                    else
                    {
                        return "Cấu kiện này không có PartCut nào.";
                    }
                }
                else
                {
                    return "Đối tượng chọn không hợp lệ.";
                }
            }
            catch (Exception ex)
            {
                return "Đã hủy hộp thoại hoặc có lỗi: " + ex.Message;
            }
        }

        public string JoinAssembly()
        {
            if (!_teklaModel.IsConnected())
            {
                return "Lỗi: Không tìm thấy Tekla Structures đang mở.";
            }

            try
            {
                Tekla.Structures.Model.UI.Picker picker = new Tekla.Structures.Model.UI.Picker();

                ModelObject mainObj = picker.PickObject(Tekla.Structures.Model.UI.Picker.PickObjectEnum.PICK_ONE_PART, "Vui lòng chọn cấu kiện chính (Main Part)...");
                if (mainObj is Part mainPart)
                {
                    ModelObjectEnumerator secondaryObjects = picker.PickObjects(Tekla.Structures.Model.UI.Picker.PickObjectsEnum.PICK_N_PARTS, "Quét chọn các cấu kiện phụ, click chuột GIỮA để hoàn tất...");

                    Tekla.Structures.Model.Assembly assembly = mainPart.GetAssembly();
                    int count = 0;

                    while (secondaryObjects.MoveNext())
                    {
                        if (secondaryObjects.Current is Part secPart && secPart.Identifier.ID != mainPart.Identifier.ID)
                        {
                            assembly.Add(secPart);
                            count++;
                        }
                    }

                    if (count > 0)
                    {
                        assembly.Modify();
                        _teklaModel.Commit();
                        return $"Hoàn tất!\r\nĐã thêm thành công {count} cấu kiện phụ vào Assembly/CastUnit của cấu kiện chính (Profile: {mainPart.Profile.ProfileString}).";
                    }
                    else
                    {
                        return "Không có cấu kiện phụ nào hợp lệ được chọn thêm.";
                    }
                }
                else
                {
                    return "Đối tượng chọn làm Main Part không hợp lệ (Không phải là Part).";
                }
            }
            catch (Exception ex)
            {
                return "Đã hủy chọn hoặc có lỗi xảy ra: " + ex.Message;
            }
        }
        public string RemoveDuplicateCuts()
        {
            if (!_teklaModel.IsConnected())
            {
                return "Lỗi: Không tìm thấy Tekla Structures đang mở.";
            }

            try
            {
                Tekla.Structures.Model.UI.Picker picker = new Tekla.Structures.Model.UI.Picker();
                ModelObject pickedObject = picker.PickObject(Tekla.Structures.Model.UI.Picker.PickObjectEnum.PICK_ONE_PART, "Vui lòng chọn cấu kiện để loại bỏ PartCut trùng");

                if (pickedObject is Part hostPart)
                {
                    ModelObjectEnumerator cutEnumerator = hostPart.GetBooleans();
                    var existingCuts = new System.Collections.Generic.Dictionary<string, BooleanPart>();
                    int deletedCount = 0;
                    int totalCuts = 0;

                    while (cutEnumerator.MoveNext())
                    {
                        if (cutEnumerator.Current is BooleanPart booleanCut)
                        {
                            totalCuts++;
                            Part cuttingPart = booleanCut.OperativePart;
                            if (cuttingPart == null) continue;

                            double volume = 0;
                            cuttingPart.GetReportProperty("VOLUME", ref volume);

                            // Lấy tâm khối của Solid để xác định vị trí
                            var solid = booleanCut.OperativePart.GetSolid();
                            double midX = Math.Round((solid.MinimumPoint.X + solid.MaximumPoint.X) / 2.0, 1);
                            double midY = Math.Round((solid.MinimumPoint.Y + solid.MaximumPoint.Y) / 2.0, 1);
                            double midZ = Math.Round((solid.MinimumPoint.Z + solid.MaximumPoint.Z) / 2.0, 1);

                            // Tạo chữ ký định danh cho vết cắt: Thể tích + Tọa độ tâm
                            string signature = $"{Math.Round(volume, 2)}_{midX}_{midY}_{midZ}";

                            if (existingCuts.ContainsKey(signature))
                            {
                                // Nếu đã tồn tại vết cắt y hệt -> Xóa cái này đi
                                booleanCut.Delete();
                                deletedCount++;
                            }
                            else
                            {
                                existingCuts.Add(signature, booleanCut);
                            }
                        }
                    }

                    if (deletedCount > 0)
                    {
                        _teklaModel.Commit();
                        return $"Thành công!\r\n- Tổng số vết cắt ban đầu: {totalCuts}\r\n- Đã xóa {deletedCount} vết cắt trùng lặp.\r\n- Giữ lại {existingCuts.Count} vết cắt duy nhất.";
                    }
                    else
                    {
                        return $"Không tìm thấy vết cắt nào trùng lặp trên cấu kiện này (Tổng số: {totalCuts}).";
                    }
                }
                else
                {
                    return "Đối tượng chọn không hợp lệ.";
                }
            }
            catch (Exception ex)
            {
                return "Lỗi khi xử lý trùng lặp: " + ex.Message;
            }
        }
    }
}
