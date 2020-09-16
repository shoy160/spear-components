/*
 Navicat Premium Data Transfer

 Source Server         : 60.255.161.101
 Source Server Type    : MySQL
 Source Server Version : 80016
 Source Host           : 60.255.161.101:3306
 Source Schema         : spear

 Target Server Type    : MySQL
 Target Server Version : 80016
 File Encoding         : 65001

 Date: 16/09/2020 18:20:27
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for t_account
-- ----------------------------
DROP TABLE IF EXISTS `t_account`;
CREATE TABLE `t_account`  (
  `id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `account` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '账号',
  `password` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '密码',
  `password_salt` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '密码盐',
  `nick` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '昵称',
  `avatar` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '头像',
  `role` tinyint(4) NOT NULL COMMENT '角色',
  `create_time` datetime(0) NOT NULL COMMENT '创建时间',
  `last_login_time` datetime(0) NULL DEFAULT NULL COMMENT '最后登录时间',
  `project_id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '项目ID',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_account
-- ----------------------------
INSERT INTO `t_account` VALUES ('03174340d3a6c64778f408d85a4d6d60', 'shay', 'E1B33E7E6CD6128C8348068F4DD24F36', '6b852476f423833e', '管理员', NULL, 127, '2020-09-16 14:33:07', NULL, NULL);
INSERT INTO `t_account` VALUES ('322396939479ca95541908d85a52c521', 'test', '1B9AFAE56158EB7E8443BBE2443047EF', '764c7bd68b91c92e', 'Test', NULL, 1, '2020-09-16 15:11:21', NULL, '323abe0282bfc175d05008d85a52a973');

-- ----------------------------
-- Table structure for t_account_record
-- ----------------------------
DROP TABLE IF EXISTS `t_account_record`;
CREATE TABLE `t_account_record`  (
  `id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `account_id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '账号Id',
  `status` tinyint(4) NOT NULL COMMENT '状态',
  `remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `create_ip` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT 'IP',
  `create_time` datetime(0) NOT NULL COMMENT '创建时间',
  `user_agent` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_config
-- ----------------------------
DROP TABLE IF EXISTS `t_config`;
CREATE TABLE `t_config`  (
  `id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '名称',
  `mode` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '模式：Dev,Test,Ready,Prod...',
  `content` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '配置内容',
  `project_id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '项目ID',
  `md5` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'MD5',
  `desc` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `status` tinyint(4) NOT NULL COMMENT '状态',
  `timestamp` timestamp(0) NOT NULL DEFAULT CURRENT_TIMESTAMP(0) ON UPDATE CURRENT_TIMESTAMP(0) COMMENT '时间戳',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_config
-- ----------------------------
INSERT INTO `t_config` VALUES ('232d6891d222c8924bbc08d85a6bf199', 'test', NULL, '{\"test\":123456}', '323abe0282bfc175d05008d85a52a973', '25BCF6655D266536CF0A080BEF6A1E23', NULL, 0, '2020-09-16 18:11:33');

-- ----------------------------
-- Table structure for t_database
-- ----------------------------
DROP TABLE IF EXISTS `t_database`;
CREATE TABLE `t_database`  (
  `id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `account_id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '账号ID',
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `code` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `provider` tinyint(4) NOT NULL COMMENT '数据提供者',
  `connection_string` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '连接字符串',
  `status` tinyint(4) NOT NULL COMMENT '状态',
  `create_time` datetime(0) NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;


-- ----------------------------
-- Table structure for t_job
-- ----------------------------
DROP TABLE IF EXISTS `t_job`;
CREATE TABLE `t_job`  (
  `id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '任务名称',
  `group` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '任务分组',
  `status` tinyint(4) NOT NULL COMMENT '状态',
  `type` tinyint(4) NOT NULL COMMENT '任务类型',
  `desc` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '任务描述',
  `create_time` datetime(0) NOT NULL COMMENT '创建时间',
  `project_id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '项目ID',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_job
-- ----------------------------
INSERT INTO `t_job` VALUES ('5d8b33b01a6cc5c9887008d85a52eecd', '访问百度', 'Test', 0, 0, NULL, '2020-09-16 15:37:43', '323abe0282bfc175d05008d85a52a973');

-- ----------------------------
-- Table structure for t_job_http
-- ----------------------------
DROP TABLE IF EXISTS `t_job_http`;
CREATE TABLE `t_job_http`  (
  `id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `url` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'URL地址',
  `method` int(255) NOT NULL COMMENT '请求方式',
  `body_type` int(255) NOT NULL COMMENT '数据类型',
  `header` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '请求头',
  `data` varchar(2048) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '请求数据',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_job_http
-- ----------------------------
INSERT INTO `t_job_http` VALUES ('5d8b33b01a6cc5c9887008d85a52eecd', 'http://www.baidu.com', 0, 0, NULL, NULL);

-- ----------------------------
-- Table structure for t_job_record
-- ----------------------------
DROP TABLE IF EXISTS `t_job_record`;
CREATE TABLE `t_job_record`  (
  `id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `job_id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '任务ID',
  `status` tinyint(4) NOT NULL COMMENT '状态',
  `start_time` datetime(0) NOT NULL COMMENT '开始时间',
  `complete_time` datetime(0) NOT NULL COMMENT '完成时间',
  `remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `trigger_id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '触发器ID',
  `result` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT '执行结果',
  `result_code` int(11) NOT NULL COMMENT '状态码',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_job_trigger
-- ----------------------------
DROP TABLE IF EXISTS `t_job_trigger`;
CREATE TABLE `t_job_trigger`  (
  `id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `job_id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '任务Id',
  `type` tinyint(4) NOT NULL COMMENT '触发器类型',
  `corn` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT 'Corn表达式',
  `times` int(11) NULL DEFAULT NULL COMMENT '执行次数',
  `interval` int(255) NULL DEFAULT NULL COMMENT '执行间隔(秒)',
  `start` datetime(0) NULL DEFAULT NULL COMMENT '开始时间',
  `expired` datetime(0) NULL DEFAULT NULL COMMENT '结束时间',
  `prev_time` datetime(0) NULL DEFAULT NULL COMMENT '上次执行时间',
  `status` tinyint(4) NOT NULL COMMENT '状态',
  `create_time` datetime(0) NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_job_trigger
-- ----------------------------
INSERT INTO `t_job_trigger` VALUES ('762ead2c36edcca479cb08d85a56a3ec', '5d8b33b01a6cc5c9887008d85a52eecd', 2, NULL, 0, 10, NULL, NULL, '2020-09-16 17:33:03', 1, '2020-09-16 15:39:04');

-- ----------------------------
-- Table structure for t_project
-- ----------------------------
DROP TABLE IF EXISTS `t_project`;
CREATE TABLE `t_project`  (
  `id` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '项目名称',
  `security` tinyint(4) NOT NULL COMMENT '安全等级：0,匿名;1,管理验证;2.获取验证;',
  `desc` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '项目描述',
  `code` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '项目编码',
  `secret` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '项目秘钥',
  `create_time` datetime(0) NOT NULL COMMENT '创建时间',
  `status` tinyint(4) NOT NULL COMMENT '状态',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_project
-- ----------------------------
INSERT INTO `t_project` VALUES ('323abe0282bfc175d05008d85a52a973', 'test', 0, 'test', 'test', 'b45081cdae136a93', '2020-09-16 15:10:35', 0);

SET FOREIGN_KEY_CHECKS = 1;
