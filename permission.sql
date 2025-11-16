------------------------------------------------------------
-- PERMISSION SYSTEM SEED DATA - EXACT COLUMN NAMES
-- Based on actual MDUA database schema
------------------------------------------------------------

------------------------------------------------------------
-- 1️⃣ USER LOGIN - Create Admin Users
------------------------------------------------------------
INSERT INTO UserLogin (UserName, Email, Phone, Password, CompanyId, CreatedBy, CreatedAt)
VALUES 
('admin', 'admin@hoodiehouse.com', '01780620311', 'Admin@123', 1, 'system', SYSDATETIME()),
('manager', 'manager@hoodiehouse.com', '01780620312', 'Manager@123', 1, 'system', SYSDATETIME()),
('staff', 'staff@hoodiehouse.com', '01780620313', 'Staff@123', 1, 'system', SYSDATETIME());
-- Note: These are plain text passwords - implement proper hashing in production!

DECLARE @AdminUserId INT = (SELECT Id FROM UserLogin WHERE UserName = 'admin');
DECLARE @ManagerUserId INT = (SELECT Id FROM UserLogin WHERE UserName = 'manager');
DECLARE @StaffUserId INT = (SELECT Id FROM UserLogin WHERE UserName = 'staff');


------------------------------------------------------------
-- 2️⃣ PERMISSION - Define System Permissions
------------------------------------------------------------
INSERT INTO Permission (Name)
VALUES 
-- Product Management
('Product.View'),
('Product.Add'),
('Product.Edit'),
('Product.Delete'),
('Product.Manage'),

-- Variant Management
('Variant.View'),
('Variant.Add'),
('Variant.Edit'),
('Variant.Delete'),

-- Order Management
('Order.View'),
('Order.Process'),
('Order.Cancel'),
('Order.Detail'),
('Order.Export'),

-- User Management
('User.View'),
('User.Add'),
('User.Edit'),
('User.Delete'),
('User.Manage'),

-- Category Management
('Category.View'),
('Category.Add'),
('Category.Edit'),
('Category.Delete'),

-- Inventory Management
('Inventory.View'),
('Inventory.Update'),
('Inventory.Adjust'),

-- Attribute Management
('Attribute.View'),
('Attribute.Manage'),

-- Reports & Analytics
('Report.View'),
('Report.Export'),
('Analytics.View'),

-- System Settings
('Settings.View'),
('Settings.Manage'),

-- Company Management
('Company.View'),
('Company.Manage');

-- Store Permission IDs
DECLARE @PermProductView INT = (SELECT Id FROM Permission WHERE Name = 'Product.View');
DECLARE @PermProductAdd INT = (SELECT Id FROM Permission WHERE Name = 'Product.Add');
DECLARE @PermProductEdit INT = (SELECT Id FROM Permission WHERE Name = 'Product.Edit');
DECLARE @PermProductDelete INT = (SELECT Id FROM Permission WHERE Name = 'Product.Delete');
DECLARE @PermProductManage INT = (SELECT Id FROM Permission WHERE Name = 'Product.Manage');
DECLARE @PermVariantView INT = (SELECT Id FROM Permission WHERE Name = 'Variant.View');
DECLARE @PermVariantAdd INT = (SELECT Id FROM Permission WHERE Name = 'Variant.Add');
DECLARE @PermVariantEdit INT = (SELECT Id FROM Permission WHERE Name = 'Variant.Edit');
DECLARE @PermVariantDelete INT = (SELECT Id FROM Permission WHERE Name = 'Variant.Delete');
DECLARE @PermOrderView INT = (SELECT Id FROM Permission WHERE Name = 'Order.View');
DECLARE @PermOrderProcess INT = (SELECT Id FROM Permission WHERE Name = 'Order.Process');
DECLARE @PermOrderCancel INT = (SELECT Id FROM Permission WHERE Name = 'Order.Cancel');
DECLARE @PermOrderDetail INT = (SELECT Id FROM Permission WHERE Name = 'Order.Detail');
DECLARE @PermOrderExport INT = (SELECT Id FROM Permission WHERE Name = 'Order.Export');
DECLARE @PermUserView INT = (SELECT Id FROM Permission WHERE Name = 'User.View');
DECLARE @PermUserAdd INT = (SELECT Id FROM Permission WHERE Name = 'User.Add');
DECLARE @PermUserEdit INT = (SELECT Id FROM Permission WHERE Name = 'User.Edit');
DECLARE @PermUserDelete INT = (SELECT Id FROM Permission WHERE Name = 'User.Delete');
DECLARE @PermUserManage INT = (SELECT Id FROM Permission WHERE Name = 'User.Manage');
DECLARE @PermCategoryView INT = (SELECT Id FROM Permission WHERE Name = 'Category.View');
DECLARE @PermCategoryAdd INT = (SELECT Id FROM Permission WHERE Name = 'Category.Add');
DECLARE @PermCategoryEdit INT = (SELECT Id FROM Permission WHERE Name = 'Category.Edit');
DECLARE @PermCategoryDelete INT = (SELECT Id FROM Permission WHERE Name = 'Category.Delete');
DECLARE @PermInventoryView INT = (SELECT Id FROM Permission WHERE Name = 'Inventory.View');
DECLARE @PermInventoryUpdate INT = (SELECT Id FROM Permission WHERE Name = 'Inventory.Update');
DECLARE @PermInventoryAdjust INT = (SELECT Id FROM Permission WHERE Name = 'Inventory.Adjust');
DECLARE @PermAttributeView INT = (SELECT Id FROM Permission WHERE Name = 'Attribute.View');
DECLARE @PermAttributeManage INT = (SELECT Id FROM Permission WHERE Name = 'Attribute.Manage');
DECLARE @PermReportView INT = (SELECT Id FROM Permission WHERE Name = 'Report.View');
DECLARE @PermReportExport INT = (SELECT Id FROM Permission WHERE Name = 'Report.Export');
DECLARE @PermAnalyticsView INT = (SELECT Id FROM Permission WHERE Name = 'Analytics.View');
DECLARE @PermSettingsView INT = (SELECT Id FROM Permission WHERE Name = 'Settings.View');
DECLARE @PermSettingsManage INT = (SELECT Id FROM Permission WHERE Name = 'Settings.Manage');
DECLARE @PermCompanyView INT = (SELECT Id FROM Permission WHERE Name = 'Company.View');
DECLARE @PermCompanyManage INT = (SELECT Id FROM Permission WHERE Name = 'Company.Manage');


------------------------------------------------------------
-- 3️⃣ PERMISSION GROUP - Create Role Groups
------------------------------------------------------------
INSERT INTO PermissionGroup (Name)
VALUES 
('Super Admin'),
('Product Manager'),
('Order Manager'),
('Inventory Manager'),
('Staff');

DECLARE @GroupSuperAdmin INT = (SELECT Id FROM PermissionGroup WHERE Name = 'Super Admin');
DECLARE @GroupProductManager INT = (SELECT Id FROM PermissionGroup WHERE Name = 'Product Manager');
DECLARE @GroupOrderManager INT = (SELECT Id FROM PermissionGroup WHERE Name = 'Order Manager');
DECLARE @GroupInventoryManager INT = (SELECT Id FROM PermissionGroup WHERE Name = 'Inventory Manager');
DECLARE @GroupStaff INT = (SELECT Id FROM PermissionGroup WHERE Name = 'Staff');


------------------------------------------------------------
-- 4️⃣ PERMISSION GROUP MAP - Assign Permissions to Groups
------------------------------------------------------------

-- Super Admin Group - ALL PERMISSIONS
INSERT INTO PermissionGroupMap (PermissionId, PermissionGroupId)
SELECT Id, @GroupSuperAdmin FROM Permission;

-- Product Manager Group
INSERT INTO PermissionGroupMap (PermissionId, PermissionGroupId)
VALUES 
(@PermProductView, @GroupProductManager),
(@PermProductAdd, @GroupProductManager),
(@PermProductEdit, @GroupProductManager),
(@PermProductDelete, @GroupProductManager),
(@PermProductManage, @GroupProductManager),
(@PermVariantView, @GroupProductManager),
(@PermVariantAdd, @GroupProductManager),
(@PermVariantEdit, @GroupProductManager),
(@PermVariantDelete, @GroupProductManager),
(@PermCategoryView, @GroupProductManager),
(@PermCategoryAdd, @GroupProductManager),
(@PermCategoryEdit, @GroupProductManager),
(@PermCategoryDelete, @GroupProductManager),
(@PermAttributeView, @GroupProductManager),
(@PermAttributeManage, @GroupProductManager),
(@PermInventoryView, @GroupProductManager),
(@PermReportView, @GroupProductManager);

-- Order Manager Group
INSERT INTO PermissionGroupMap (PermissionId, PermissionGroupId)
VALUES 
(@PermOrderView, @GroupOrderManager),
(@PermOrderProcess, @GroupOrderManager),
(@PermOrderCancel, @GroupOrderManager),
(@PermOrderDetail, @GroupOrderManager),
(@PermOrderExport, @GroupOrderManager),
(@PermProductView, @GroupOrderManager),
(@PermInventoryView, @GroupOrderManager),
(@PermReportView, @GroupOrderManager),
(@PermReportExport, @GroupOrderManager),
(@PermAnalyticsView, @GroupOrderManager);

-- Inventory Manager Group
INSERT INTO PermissionGroupMap (PermissionId, PermissionGroupId)
VALUES 
(@PermInventoryView, @GroupInventoryManager),
(@PermInventoryUpdate, @GroupInventoryManager),
(@PermInventoryAdjust, @GroupInventoryManager),
(@PermProductView, @GroupInventoryManager),
(@PermVariantView, @GroupInventoryManager),
(@PermReportView, @GroupInventoryManager);

-- Staff Group - Read-only access
INSERT INTO PermissionGroupMap (PermissionId, PermissionGroupId)
VALUES 
(@PermProductView, @GroupStaff),
(@PermVariantView, @GroupStaff),
(@PermOrderView, @GroupStaff),
(@PermOrderDetail, @GroupStaff),
(@PermCategoryView, @GroupStaff),
(@PermInventoryView, @GroupStaff);


------------------------------------------------------------
-- 5️⃣ USER PERMISSION - Assign Users to Groups
------------------------------------------------------------

-- Admin User - Super Admin Group (All Permissions)
INSERT INTO UserPermission (UserId, PermissionId, PermissionGroupId)
SELECT @AdminUserId, Id, @GroupSuperAdmin
FROM Permission;

-- Manager User - Product Manager Group
INSERT INTO UserPermission (UserId, PermissionId, PermissionGroupId)
SELECT @ManagerUserId, PermissionId, @GroupProductManager
FROM PermissionGroupMap 
WHERE PermissionGroupId = @GroupProductManager;

-- Staff User - Staff Group
INSERT INTO UserPermission (UserId, PermissionId, PermissionGroupId)
SELECT @StaffUserId, PermissionId, @GroupStaff
FROM PermissionGroupMap 
WHERE PermissionGroupId = @GroupStaff;


------------------------------------------------------------
-- 🔎 VERIFICATION QUERIES
------------------------------------------------------------

PRINT '=== USERS ===';
SELECT Id, UserName, Email, Phone, CompanyId FROM UserLogin;

PRINT '=== PERMISSIONS ===';
SELECT Id, Name FROM Permission ORDER BY Name;

PRINT '=== PERMISSION GROUPS ===';
SELECT Id, Name FROM PermissionGroup;

PRINT '=== GROUP PERMISSIONS ===';
SELECT 
    pg.Name AS GroupName,
    p.Name AS Permission,
    COUNT(*) OVER (PARTITION BY pg.Id) AS TotalPermissions
FROM PermissionGroupMap pgm
JOIN PermissionGroup pg ON pgm.PermissionGroupId = pg.Id
JOIN Permission p ON pgm.PermissionId = p.Id
ORDER BY pg.Name, p.Name;

PRINT '=== USER PERMISSIONS ===';
SELECT 
    ul.UserName,
    pg.Name AS PermissionGroup,
    p.Name AS Permission
FROM UserPermission up
JOIN UserLogin ul ON up.UserId = ul.Id
JOIN Permission p ON up.PermissionId = p.Id
JOIN PermissionGroup pg ON up.PermissionGroupId = pg.Id
ORDER BY ul.UserName, p.Name;


------------------------------------------------------------
-- 📝 SUMMARY
------------------------------------------------------------
/*
✅ Created 3 Users:
   - admin (Super Admin) - All permissions
   - manager (Product Manager) - Product, Category, Inventory management
   - staff (Staff) - Read-only access

✅ Created 37 Permissions covering:
   - Product Management (5)
   - Variant Management (4)
   - Order Management (5)
   - User Management (5)
   - Category Management (4)
   - Inventory Management (3)
   - Attribute Management (2)
   - Reports & Analytics (3)
   - System Settings (2)
   - Company Management (2)

✅ Created 5 Permission Groups:
   - Super Admin (all permissions)
   - Product Manager (17 permissions)
   - Order Manager (10 permissions)
   - Inventory Manager (6 permissions)
   - Staff (6 permissions - read-only)

🔐 Default Passwords:
   - admin: Admin@123
   - manager: Manager@123
   - staff: Staff@123
   
⚠️ IMPORTANT: Implement password hashing before production!
*/