---
title: "v2.x to 3.x migration"
permalink: /docs/v2-to-v3/
---

Make sure you have gone through [openiddict 5.0 general availability guide](https://kevinchalet.com/2023/12/18/openiddict-5-0-general-availability/) and 
[v4.0-v5.0 OpenIdDict migration guide](https://documentation.openiddict.com/guides/migration/40-to-50.html)

### Migrating from v2.x to 3.x when using Mongo plugin
1. Backup your database
2. See [update your openiddict mongodb applications](https://documentation.openiddict.com/guides/migration/40-to-50.html#if-applicable-update-your-openiddict-mongodb-applications) . Same code is provided as a project [Pixel.Identity.Store.Mongo.Migrations](https://github.com/Nfactor26/pixel-identity/tree/main/src/Pixel.Identity.Store.Mongo.Migrations). You can build this and run the app by setting up mongo-host and mongo-db in the appsetttings.json file to update your database.
3. Alternatively you can use any other method to do the same. For example, you can execute below after connecting to your mongo database from robo-3t

        db.getCollection('openiddict.applications').updateMany( {}, { $rename: { "type": "client_type" } } )


### Migrating from v2.x to 3.x when using SQL Server or Postgre SQL plugin
1. Backup your database
2. Download the SQL files provided with v3 release. See https://github.com/Nfactor26/pixel-identity/releases
   - Execute script PostgreSql_v_2_0_to_v_3_0.sql after connecting to your database from your choice of editor if you are using PostgreSQL
   - Execute script SqlServer_v_2_0_to_v_3_0.sql after connecting to your database from your choice of editor if you are using SQL server