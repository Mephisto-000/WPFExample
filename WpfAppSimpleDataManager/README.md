# WpfAppSimpleDataManager

一個極簡的 WPF MVVM 檔案管理工具，支援 CSV 與 Excel (XLSX) 檔案的建立、開啟、儲存、刪除，並可於建立時動態指定欄位數。適合教學、專案練習，或作為類似工具開發的 MVVM 架構範本。

---

## 功能特色

- 支援 CSV、Excel (XLSX) 檔案的 CRUD（建立、讀取、寫入、刪除）
- 新增檔案時可自訂要建立幾個欄位（Column）
- 以 TabControl 分頁管理 CSV 和 Excel 功能
- 介面簡潔，完全採用 MVVM 架構
- 完全支援動態欄位數、不限定 schema

---

## 專案資料夾結構說明

```text
WpfFileManager/
│
├─ Views/             # 所有 UI 畫面（主視窗、分頁、對話視窗）
│    ├─ MainWindow.xaml / .cs
│    ├─ CsvView.xaml / .cs
│    ├─ ExcelView.xaml / .cs
│    └─ InputDialog.xaml / .cs
│
├─ ViewModels/        # 畫面背後的邏輯與資料繫結
│    ├─ MainWindowViewModel.cs
│    ├─ CsvViewModel.cs
│    ├─ ExcelViewModel.cs
│    └─ RelayCommand.cs
│
├─ Services/          # 負責與檔案系統溝通的層（檔案 CRUD）
│    ├─ ICsvService.cs
│    ├─ CsvService.cs
│    ├─ IExcelService.cs
│    └─ ExcelService.cs
│
├─ App.xaml           # 應用啟動與資源設定
├─ App.xaml.cs
└─ ...（其他 WPF 相關檔案）
```

**為什麼沒有 Models/ 資料夾？**
本專案屬於通用檔案編輯工具，欄位數、資料型態不固定。
直接以 DataTable 作為資料來源，可對應任意結構，無需特定 Model 類別。
若未來要加入特定商業邏輯或固定 schema，可以再擴充 Models。

## 使用到的 NuGet 套件

**ClosedXML**
用於 Excel (XLSX) 檔案的讀取與寫入。

**DocumentFormat.OpenXml**
為 ClosedXML 內部依賴，會自動安裝。
