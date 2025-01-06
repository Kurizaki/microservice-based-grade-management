INSERT INTO Users (Username, PasswordHash, IsAdmin)
SELECT 'admin', '$2y$10$G/7T0aVJhZIzmBrLl4mvP.QjORG5u5CXRT9hgW0BEwAUuAnFtk80S', 1
WHERE NOT EXISTS (
    SELECT 1 FROM Users WHERE Username = 'admin'
);
