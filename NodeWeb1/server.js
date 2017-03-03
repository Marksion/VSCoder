'use strict'

const hapi = require('hapi')
const server = new hapi.Server()

server.connection({
    host: 'localhost',
    port: 8000
})

server.route({
    method: 'GET',
    path: '/',
    handler: (request, reply) => {
        reply('hello hapi')
    }
})

server.start((err) => {
    if (err)
        throw err
    console.log('Server running at:', server.info.uri)
}) 
