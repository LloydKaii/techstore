# TechStore Role/Pagination/Animation Update - TODO

## Progress: 2/7 ✅

### 1. ✅ Update Repositories
   - IProductRepository/EFProductRepository: Add GetPagedAsync ✅
   - ICategoryRepository: Add GetPublicCategoriesAsync ✅

### 2. ✅ Create ViewModels
   - PagedViewModel.cs ✅

### 3. ✅ Update Controllers - Roles & Pagination
   - CategoryController: Public Index + role-check ✅
   - Product/HomeController: Pagination/filter ✅
   - AccountController: Role detect for Profile ✅

### 4. [ ] Update Views - Core
   - CategoryController: Public Index() for Customer; role-checks
   - ProductController/HomeController: Paged/filtered lists by role
   - AccountController: Profile() detect role → role-specific view

### 4. ✅ Update Views - Core
   - Views/Category/Index.cshtml: Role conditional ✅
   - Views/Product/Index.cshtml: Filters/pagination ✅
   - Views/Home/Index.cshtml: Pagination/filter ✅

### 5. ✅ Role-Specific Profiles
   - Created Profile-Admin/Manager/Customer.cshtml ✅
   - Profile.cshtml → role partial ✅

### 6. [ ] Animations & UI
   - _Layout.cshtml: AOS CDN
   - site.css: AOS classes/keyframe
   - JS: Filter handlers

### 7. [ ] Test & Demo
   - dotnet run
   - Test roles/pagination


### 5. [ ] Role-Specific Profiles

### 5. [ ] Role-Specific Profiles
   - Create Views/Account/Profile-Admin.cshtml, Profile-Manager.cshtml, Profile-Customer.cshtml
   - Update Profile.cshtml → @if role render partial

### 6. [ ] Animations & UI
   - _Layout.cshtml: AOS CDN
   - site.css: Keyframes, AOS classes on cards/buttons
   - JS: Filter/pagination handlers

### 7. [ ] Test & Cleanup
   - dotnet run
   - Test roles: Customer (view-only), Admin (full)
   - Update TODO as complete

**Next Step**: Confirm với user trước khi bắt đầu step 1 (Repos).

