  
  # products
	id INTEGER PRIMARY KEY, 
	orderNumber TEXT,
	orderDate TEXT,
	orderReleaseDate TEXT,
	photoBlob BLOB,
	customerId INTEGER,
	FOREIGN KEY(customerId) REFERENCES customers(id)
# files
	fileId INTEGER PRIMARY KEY,
	fileName TEXT,
	fileBlob BLOB
# product_files
	orderId INTEGER,
	fileId INTEGER,
	FOREIGN KEY(orderId) REFERENCES products(id),
	FOREIGN KEY(photoId) REFERENCES files(photoId)
#product photos
	orderId INTEGER,
    photoId INTEGER,
    FOREIGN KEY(orderId) REFERENCES your_table_name(id),
    FOREIGN KEY(photoId) REFERENCES photos(photoId)
# Custumers
	id INTEGER PRIMARY KEY, 
	name TEXT, 
	address TEXT,
	tajNumber TEXT UNIQUE
# Settings
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	setting_name TEXT NOT NULL,
	setting_value TEXT NOT NULL
# Backup
	id INTEGER PRIMARY KEY,
	location TEXT NOT NULL,
	name TEXT NOT NULL,
	date TEXT NOT NULL
