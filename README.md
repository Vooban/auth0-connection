# auth0-connection
Obtains an Auth0 token and uses it to connect to a service that uses this type of authentification.

Use `app.config` to set your configurations

| key | value |
| --- | ---- |
| api_get_url | http://localhost:9010/api/v1/controller/100 |
| api_post_url | http://localhost:9010/api/v1/controller |
| api_post_json_path | json used with the POST request (default path: ../../data/api-post-body.json) |
| api_put_url | http://localhost:9010/api/v1/controller/100 |
| api_put_json_path | json used with the PUT request (default path: ../../data/api-put-body.json) |
| api_delete_url | http://localhost:9010/api/v1/controller/100 |
| auth_token_url | https://project.auth0.com/oauth/token |
| client_id | client id credential from your auth0 dashboard |
| client_secret | client secret credential from your auth0 dashboard |
| audience | choosen audience (e.g.: https://project.com/api) |
