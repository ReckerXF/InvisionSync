-- CFX Details
lua54 'on'
fx_version 'cerulean'
games {'gta5'}

-- Resource stuff
name 'InvisionSync'
description 'Adds several features integrating the Invision Powerboard software including a whitelist, and ACE group sync.'
version 'v2'
author 'Michael.#3080'

escrow_ignore {
	"config.json"
}

files {
    "Newtonsoft.Json.dll"
}

server_script 'InvisionSync.Server.net.dll'