# TechStore v2 - PC Builder & UI Fixes

## Progress: 2/6 ✅

### 1. ✅ Fix Manager quyền Product
   - ProductController Update/Delete → Admin,Manager ✅

### 2. [ ] Model PCBuild
   - PCBuild.cs ✅
   - DbContext ✅

### 3. [ ] Category Layout đẹp

   - ProductController: Update/Delete → [Authorize(Roles="Admin,Manager")]

### 2. [ ] Category Layout đẹp
   - Views/Category/Index.cshtml: Table → responsive grid cards with images/AOS.
   - Customer no "thêm danh mục" button.

### 3. [ ] Model PCBuild
   - Models/PCBuild.cs (UserId, CPUId, RAMId, VGAId, MainId, CaseId, PSUId, TotalPrice).

### 4. [ ] BuildController & Views
   - Controllers/BuildController.cs (Index/Build/Save/List for user).
   - Views/Build/Index.cshtml (drag-drop components, real-time price).
   - Views/Build/List.cshtml (saved builds).

### 5. [ ] Auto Build AI
   - BuildController/AI (budget param → suggest components).
   - Use AIService or simple logic.

### 6. [ ] DB & Test
   - Add DbSet<PCBuild>, `dotnet ef migrations add PCBuild`, update.
   - Test all.

**Note**: Linh kiện filter by Category "Linh kiện".

