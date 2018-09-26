# Vehicle-Tracking-API
Vehicle Tracking API

#How to setup:
1. Install Couchbase Server. You can get Couchbase Server 5.1.1 Community via https://www.couchbase.com/downloads
2. Make sure the configuration for Couchbase Server matched with section "Couchbase" from appsetting.json in the solution. You can leave it as the default value for server URL : http://localhost:8091
3. Set password for Administrator : VehicleTrackingAPI or if you use other password, make sure you update the value for "VehicleBucketPassword" and "LocationBucketPassword" from appsetting.json in the solution.
4. Setup 2 buckets 
	4.1 Bucket Location
	- Go to http://127.0.0.1:8091/ui/index.html#!/buckets
	- Click 'ADD BUCKET' 
	  Bucket name : Location
	  Bucket type : ephemeral
	- Go to http://127.0.0.1:8091/ui/index.html#!/security/userRoles?pageSize=10
	- Click 'ADD USER'
	  Username : Location
	  Password : VehicleTrackingAPI
	  Role     : Admin
	4.2 Bucket Vehicle
	- Go to http://127.0.0.1:8091/ui/index.html#!/buckets
	- Click 'ADD BUCKET' 
	  Bucket name : Vehicle
	  Bucket type : Couchbase
	- Go to http://127.0.0.1:8091/ui/index.html#!/security/userRoles?pageSize=10
	- Click 'ADD USER'
	  Username : Vehicle
	  Password : VehicleTrackingAPI
	  Role     : Admin
5. Create indexes             
- Go to http://127.0.0.1:8091/ui/index.html#!/query/workbench
- Execute the query one by one 
	+ create index ix_type on Vehicle(type)
	+ create index ix_username on Vehicle(username)
	+ create index ix_vehicleId on Vehicle(vehicleId)
	+ create index ix_deviceId on Vehicle(deviceId)
	
#How to consume api in AdminController
- All requests go to AdminController need to be authenticated. In this case, you need to get token first by calling '/api/Token', then use bearer token for '/api/Admin/latestlocation' and '/api/Admin/getlocations'.
- The dummy data for Admin account will be automatically created when application starts. You can use username/password: admin/admin to get token.