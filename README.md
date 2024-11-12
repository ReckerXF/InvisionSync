# InvisionSync
*InvisionSync is a clean way to synchronize your Invision Powerboard forum groups with your FiveM ACE permissions in-game (& whitelist users, too!)*

*This script has been tested extensively in a production environment running FiveM version Cerulean and IPS version 4.7. It was originally created for my community, but I have since decided to share it with the world!*

**Why?**
-
Invision Powerboard is hands down one of the most utilized forum softwares in FiveM. While my community was in the development stage, we wanted an easy way to manage member permissions without having to manually edit the permissions file everytime someone got promoted or demoted. This manual editing would also require a server restart which is a huge security risk. So, I set out to create InvisonSync to handle permissions management for us! As a bonus, InvisionSync also has the capability to whitelist users to the server too dependent on forum group.

**Features**
- 
* Synchronize permissions via ACE from your Invision Powerboard forum to in-game.
* Whitelist forum groups to be able to join your server.
* HIGHLY customizable (see below). Also includes a debug mode for the script to tell you everything it is doing.
* ***No server hitches!***
* Does not require a server restart for permissions to be applied/revoked.

**How it works - Synchronizing Permissions**
-
1. User joins the server.
2. InvisionSync runs a check to see if they have their Steam Hex on the website and if it matches the Steam Hex on their client.
3. If they do, it gathers all user data including display name, primary groups, secondary groups, and their steam hex. If not, it sets them to the default group provided in the config.
4. InvisionSync sets their ACE group dependent on their forum groups. You can optionally check their secondary groups.
4. Upon leaving the server, InvisionSync sets them back to the default group. This allows you to demote them if needed and they will not retain the same permissions next time they try to join.

**How it works - Whitelisting**
-
1. User joins the server.
2. InvisionSync runs a check to see if they have their Steam Hex on the website and if it matches the Steam Hex on their client.
3. If they have one of the whitelisted groups provided in the config.json, they are allowed to join. If not, they are denied with a unwhitelisted message provided in the config.json.

**Setup**
-
Setup:

1. Create a rest API key in your Forum admin panel.

https://yourwebsitehere.com/admin/?app=core&module=applications&controller=api&tab=apiKeys

2. Ensure your rest API key has the following endpoint permissions:
(Enable GET /core/members and /core/members/{id})

3. Place your newly generated API key in the config.json of InvisionSync.

4. Create a user field on your forum named "Steam Hex". This is where users will put their Steam Hex for the application to pick it up.

5. Setup your ACE groups as desired.
- The config.json will have placeholders to guide you.
- If you are still confused for the groups part, it's formatted as ``"ace.group": [ forumgroupid1, forumgroupid2, etc.]``

6. In your server.cfg or other .cfg file, add the following.
``add_ace resource.invisionsync command.add_principal allow``

You're ready to start having permissions and whitelisting be sync'd!

**Support**
-
Support will be provided as requested.
