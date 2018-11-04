
1. Create tables and procedures from SQL.txt file
2. Project > Controllers > PostController > Setup Connection String  (variable saconstring)



- API URLs
	- Get Blog Post - http://localhost:57891/api/Posts/post/{title}    (background code create slug from title and search by slug)
	
	- List Blog Posts 
		- without tag - http://localhost:57891/api/Posts/posts?
		- with tag    - http://localhost:57891/api/Posts/posts?tag={tag}
	
	- List of tags  - http://localhost:57891/api/Posts/tags

	- Insert post   - http://localhost:57891/api/Posts/insert/{title}/{description}/{body}/{taglist?}

	- Update post   - http://localhost:57891/api/Posts/update/{title_old?}/{title_new?}/{description?}/{body?}
			- http://localhost:57891/api/Posts/update?title_old={old_title}&body={test_body}   (example: update ony body)

	- Delete post   - http://localhost:57891/api/Posts/delete/{title}     (background code create slug from title and delete post by slug)
