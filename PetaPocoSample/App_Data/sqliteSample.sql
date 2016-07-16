/*
Navicat SQLite Data Transfer

Source Server         : sqliteSample
Source Server Version : 30802
Source Host           : :0

Target Server Type    : SQLite
Target Server Version : 30802
File Encoding         : 65001

Date: 2016-07-16 18:42:34
*/

PRAGMA foreign_keys = OFF;

-- ----------------------------
-- Table structure for article
-- ----------------------------
DROP TABLE IF EXISTS "main"."article";
CREATE TABLE "article" (
"article_id"  INTEGER NOT NULL,
"title"  TEXT,
"date_created"  DateTime,
"draft"  Bool,
"content"  TEXT,
PRIMARY KEY ("article_id")
);
