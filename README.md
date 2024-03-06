#Blog API
This repository contains the source code for a simple Blog API built using ASP.NET Core. The API provides endpoints to manage blog posts, comments, reactions, tags, post-tag relationships, and user authentication.

##Getting Started
To run the API locally or deploy it to a server, follow the steps below:

##Prerequisites
Make sure you have the following software installed:

.NET Core SDK
Git
Clone the Repository
git clone https://github.com/AbdulRafeyMubasshir/Blog-API.git
cd blog-api
##Database Configuration
The API uses Entity Framework Core to interact with the database. Update the connection string in the appsettings.json file with your database credentials.

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=Blog;User=root;Password=mysqlpassword;Persist Security Info=False;"
},
##Run Migrations
Apply database migrations to create the necessary tables.

dotnet ef database update
##Run the API

dotnet run
The API will be available at https://localhost:5001 (HTTPS) or http://localhost:5000 (HTTP).

#API Endpoints
##Posts
GET /api/posts: Get all blog posts.
GET /api/posts/{id}: Get a specific blog post by ID.
PUT /api/posts/{id}: Update a blog post by ID.
POST /api/posts: Create a new blog post.
DELETE /api/posts/{id}: Delete a blog post by ID.
POST /api/posts/{postId}/comments: Add a comment to a blog post.
GET /api/posts/{postId}/comments: Get comments for a specific blog post.
POST /api/posts/{postId}/reactions: Add a reaction to a blog post.
GET /api/posts/{postId}/reactions: Get reactions for a specific blog post.
GET /api/posts/{postId}/tags: Get tags associated with a specific blog post.
##Comments
GET /api/comments: Get all comments.
GET /api/comments/{id}: Get a specific comment by ID.
PUT /api/comments/{id}: Update a comment by ID.
DELETE /api/comments/{id}: Delete a comment by ID.
##Reactions
GET /api/reactions: Get all reactions.
GET /api/reactions/{id}: Get a specific reaction by ID.
PUT /api/reactions/{id}: Update a reaction by ID.
POST /api/reactions: Add a new reaction.
DELETE /api/reactions/{id}: Delete a reaction by ID.
##Tags
GET /api/tags: Get all tags.
GET /api/tags/{id}: Get a specific tag by ID.
PUT /api/tags/{id}: Update a tag by ID.
POST /api/tags: Add a new tag.
DELETE /api/tags/{id}: Delete a tag by ID.
GET /api/tags/{id}/posts: Get posts associated with a specific tag.
##PostTags
GET /api/posttags: Get all post-tag relationships.
GET /api/posttags/{id}: Get a specific post-tag relationship by ID.
PUT /api/posttags/{id}: Update a post-tag relationship by ID.
POST /api/posttags: Add a new post-tag relationship.
DELETE /api/posttags/{id}: Delete a post-tag relationship by ID.
##User Authentication
POST /api/account/register: Register a new user.
GET /api/account/verify-email: Verify user email after registration.
POST /api/account/login: Authenticate and generate a JWT token.
POST /api/account/logout: Log out the authenticated user.
##Authentication
Some endpoints require authentication. Make sure to include a valid JWT token in the request headers.

##Contributing
If you'd like to contribute to this project, please follow the Contribution Guidelines.

##License
This project is licensed under the MIT License - see the LICENSE file for details.
