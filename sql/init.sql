-- --------------------------------------------------------
-- 主机:                           127.0.0.1
-- 服务器版本:                        5.5.5-10.0.14-MariaDB - mariadb.org binary distribution
-- 服务器操作系统:                      Win64
-- HeidiSQL 版本:                  7.0.0.4392
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 导出 tast 的数据库结构
DROP DATABASE IF EXISTS `tast`;
CREATE DATABASE IF NOT EXISTS `tast` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `tast`;


-- 导出  表 tast.PeroidExtermaIndex 结构
DROP TABLE IF EXISTS `PeroidExtermaIndex`;
CREATE TABLE IF NOT EXISTS `PeroidExtermaIndex` (
  `IndexId` char(32) NOT NULL COMMENT '编号',
  `HistoryId` char(32) NOT NULL COMMENT '历史编号',
  `Code` varchar(10) NOT NULL COMMENT '股票编号',
  `Peroid` int(11) NOT NULL COMMENT '区间',
  `Date` char(8) NOT NULL COMMENT '日期',
  `Min` decimal(10,4) NOT NULL COMMENT '最小值',
  `Max` decimal(10,4) NOT NULL COMMENT '最大值',
  PRIMARY KEY (`IndexId`),
  KEY `Code_Peroid_Date` (`Code`,`Peroid`,`Date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='区间极值指标';

-- 正在导出表  tast.PeroidExtermaIndex 的数据：~0 rows (大约)
/*!40000 ALTER TABLE `PeroidExtermaIndex` DISABLE KEYS */;
/*!40000 ALTER TABLE `PeroidExtermaIndex` ENABLE KEYS */;


-- 导出  表 tast.Stock 结构
DROP TABLE IF EXISTS `Stock`;
CREATE TABLE IF NOT EXISTS `Stock` (
  `Code` varchar(10) NOT NULL COMMENT '股票编号',
  `ChineseName` varchar(200) NOT NULL COMMENT '中文名称',
  `LastHistoryDate` char(8) NOT NULL COMMENT '最后更新日期',
  `Enable` tinyint(1) NOT NULL COMMENT '是否启用',
  PRIMARY KEY (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='股票';

-- 正在导出表  tast.Stock 的数据：~0 rows (大约)
/*!40000 ALTER TABLE `Stock` DISABLE KEYS */;
INSERT INTO `Stock` (`Code`, `ChineseName`, `LastHistoryDate`, `Enable`) VALUES
	('AAPL', '苹果', '', 1);
/*!40000 ALTER TABLE `Stock` ENABLE KEYS */;


-- 导出  表 tast.StockHistory 结构
DROP TABLE IF EXISTS `StockHistory`;
CREATE TABLE IF NOT EXISTS `StockHistory` (
  `HistoryId` char(32) NOT NULL COMMENT '编号',
  `PrevHistoryId` char(32) NOT NULL COMMENT '前一天编号',
  `Code` varchar(10) NOT NULL COMMENT '股票编号',
  `Date` char(8) NOT NULL COMMENT '日期',
  `PrevDate` char(8) NOT NULL COMMENT '上一个日期',
  `Open` decimal(10,4) NOT NULL COMMENT '开盘价',
  `High` decimal(10,4) NOT NULL COMMENT '最高价',
  `Low` decimal(10,4) NOT NULL COMMENT '最低价',
  `Close` decimal(10,4) NOT NULL COMMENT '收盘价',
  `Volume` bigint(20) NOT NULL COMMENT '交易量',
  PRIMARY KEY (`HistoryId`),
  KEY `Date` (`Date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='股票历史';

-- 正在导出表  tast.StockHistory 的数据：~0 rows (大约)
/*!40000 ALTER TABLE `StockHistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `StockHistory` ENABLE KEYS */;


-- 导出  表 tast.TurtleIndex 结构
DROP TABLE IF EXISTS `TurtleIndex`;
CREATE TABLE IF NOT EXISTS `TurtleIndex` (
  `IndexId` char(32) NOT NULL COMMENT '编号',
  `HistoryId` char(32) NOT NULL COMMENT '历史编号',
  `Code` varchar(10) NOT NULL COMMENT '股票编号',
  `Peroid` int(11) NOT NULL COMMENT '区间',
  `Date` char(8) NOT NULL COMMENT '日期',
  `N` decimal(10,4) NOT NULL COMMENT '波动性均值',
  `TR` decimal(10,4) NOT NULL COMMENT '真实波动性',
  PRIMARY KEY (`IndexId`),
  KEY `Code_Peroid_Date` (`Code`,`Peroid`,`Date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='海龟指标';

-- 正在导出表  tast.TurtleIndex 的数据：~0 rows (大约)
/*!40000 ALTER TABLE `TurtleIndex` DISABLE KEYS */;
/*!40000 ALTER TABLE `TurtleIndex` ENABLE KEYS */;


-- 导出  表 tast.TurtleTradingSystem 结构
DROP TABLE IF EXISTS `TurtleTradingSystem`;
CREATE TABLE IF NOT EXISTS `TurtleTradingSystem` (
  `SystemId` char(32) NOT NULL COMMENT '系统编号',
  `Code` varchar(10) NOT NULL COMMENT '股票编号',
  `StartAmount` decimal(10,4) NOT NULL COMMENT '起始资金',
  `Commission` decimal(10,4) NOT NULL COMMENT '佣金',
  `StartDate` char(8) NOT NULL COMMENT '起始日期',
  `EndDate` char(8) NOT NULL COMMENT '终止日期',
  `StartHolding` int(11) NOT NULL COMMENT '起始头寸',
  `StartN` int(11) NOT NULL COMMENT '起始真实波动幅度区间',
  `StartEnter` int(11) NOT NULL COMMENT '起始入市区间',
  `StartExit` int(11) NOT NULL COMMENT '起始退市区间',
  `StartStop` int(11) NOT NULL COMMENT '起始止损区间',
  `EndHolding` int(11) NOT NULL COMMENT '结束头寸',
  `EndN` int(11) NOT NULL COMMENT '结束真实波动幅度区间',
  `EndEnter` int(11) NOT NULL COMMENT '结束入市区间',
  `EndExit` int(11) NOT NULL COMMENT '结束退市区间',
  `EndStop` int(11) NOT NULL COMMENT '结束止损区间',
  `CurrentHolding` int(11) NOT NULL COMMENT '当前头寸',
  `CurrentN` int(11) NOT NULL COMMENT '当前真实波动幅度区间',
  `CurrentEnter` int(11) NOT NULL COMMENT '当前入市区间',
  `CurrentExit` int(11) NOT NULL COMMENT '当前退市区间',
  `CurrentStop` int(11) NOT NULL COMMENT '当前止损区间',
  `CurrentProfit` decimal(10,4) NOT NULL COMMENT '当前利润额',
  `CurrentProfitPercent` decimal(10,4) NOT NULL COMMENT '当前利润率',
  `BestHolding` int(11) NOT NULL COMMENT '最佳头寸',
  `BestN` int(11) NOT NULL COMMENT '最佳真实波动幅度区间',
  `BestEnter` int(11) NOT NULL COMMENT '最佳入市区间',
  `BestExit` int(11) NOT NULL COMMENT '最佳退市区间',
  `BestStop` int(11) NOT NULL COMMENT '最佳止损区间',
  `BestProfit` decimal(10,4) NOT NULL COMMENT '最佳利润额',
  `BestProfitPercent` decimal(10,4) NOT NULL COMMENT '最佳利润率',
  `Enable` tinyint(1) NOT NULL COMMENT '状态',
  `TotalAmount` bigint(20) NOT NULL COMMENT '总计算量',
  `FinishedAmount` bigint(20) NOT NULL COMMENT '已完成计算量',
  `RemainTips` varchar(200) DEFAULT NULL COMMENT '剩余时间提示',
  PRIMARY KEY (`SystemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='海龟交易系统';

-- 正在导出表  tast.TurtleTradingSystem 的数据：~0 rows (大约)
/*!40000 ALTER TABLE `TurtleTradingSystem` DISABLE KEYS */;
/*!40000 ALTER TABLE `TurtleTradingSystem` ENABLE KEYS */;


-- 导出  表 tast.TurtleTradingSystemHolding 结构
DROP TABLE IF EXISTS `TurtleTradingSystemHolding`;
CREATE TABLE IF NOT EXISTS `TurtleTradingSystemHolding` (
  `HoldingId` char(32) NOT NULL COMMENT '头寸编号',
  `SystemId` char(32) NOT NULL COMMENT '系统编号',
  `TestId` char(32) NOT NULL COMMENT '测试编号',
  `TrendId` char(32) NOT NULL COMMENT '趋势编号',
  `Direction` tinyint(1) NOT NULL COMMENT '方向: 0-多 1-空',
  `StartDate` char(8) NOT NULL COMMENT '开始日期',
  `StartPrice` decimal(10,4) NOT NULL COMMENT '开始价格',
  `EndDate` char(8) NOT NULL COMMENT '结束日期',
  `EndPrice` decimal(10,4) NOT NULL COMMENT '结束价格',
  `Quantity` int(11) NOT NULL COMMENT '数量',
  `Profit` decimal(10,4) NOT NULL COMMENT '利润',
  PRIMARY KEY (`HoldingId`),
  KEY `SystemId_TestId_TrendId` (`SystemId`,`TestId`,`TrendId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='海龟交易系统头寸';

-- 正在导出表  tast.TurtleTradingSystemHolding 的数据：~0 rows (大约)
/*!40000 ALTER TABLE `TurtleTradingSystemHolding` DISABLE KEYS */;
/*!40000 ALTER TABLE `TurtleTradingSystemHolding` ENABLE KEYS */;


-- 导出  表 tast.TurtleTradingSystemTest 结构
DROP TABLE IF EXISTS `TurtleTradingSystemTest`;
CREATE TABLE IF NOT EXISTS `TurtleTradingSystemTest` (
  `TestId` char(32) NOT NULL COMMENT '测试编号',
  `SystemId` char(32) NOT NULL COMMENT '系统编号',
  `Holding` int(11) NOT NULL COMMENT '头寸',
  `N` int(11) NOT NULL COMMENT '真实波动幅度区间',
  `Enter` int(11) NOT NULL COMMENT '入市区间',
  `Exit` int(11) NOT NULL COMMENT '退市区间',
  `Stop` int(11) NOT NULL COMMENT '止损区间',
  `StartAmount` decimal(10,4) NOT NULL COMMENT '起始资金量',
  `EndAmount` decimal(10,4) NOT NULL COMMENT '结束资金量',
  `Commission` decimal(10,4) NOT NULL COMMENT '佣金',
  `Profit` decimal(10,4) NOT NULL COMMENT '利润额',
  `ProfitPercent` decimal(10,4) NOT NULL COMMENT '利润率',
  PRIMARY KEY (`TestId`),
  KEY `SystemId` (`SystemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='海龟交易系统测试';

-- 正在导出表  tast.TurtleTradingSystemTest 的数据：~0 rows (大约)
/*!40000 ALTER TABLE `TurtleTradingSystemTest` DISABLE KEYS */;
/*!40000 ALTER TABLE `TurtleTradingSystemTest` ENABLE KEYS */;


-- 导出  表 tast.TurtleTradingSystemTrend 结构
DROP TABLE IF EXISTS `TurtleTradingSystemTrend`;
CREATE TABLE IF NOT EXISTS `TurtleTradingSystemTrend` (
  `TrendId` char(32) NOT NULL COMMENT '趋势编号',
  `SystemId` char(32) NOT NULL COMMENT '系统编号',
  `TestId` char(32) NOT NULL COMMENT '测试编号',
  `Direction` tinyint(1) NOT NULL COMMENT '方向: 0-多 1-空',
  `StartDate` char(8) NOT NULL COMMENT '开始日期',
  `StartPrice` decimal(10,4) NOT NULL COMMENT '开始价格',
  `StartReason` varchar(100) NOT NULL COMMENT '开始原因',
  `EndDate` char(8) NOT NULL COMMENT '结束日期',
  `EndPrice` decimal(10,4) NOT NULL COMMENT '结束价格',
  `EndReason` varchar(100) NOT NULL COMMENT '结束原因',
  `MaxHolding` int(11) NOT NULL COMMENT '最大头寸',
  `Profit` decimal(10,4) NOT NULL COMMENT '利润',
  PRIMARY KEY (`TrendId`),
  KEY `SystemId_TestId` (`SystemId`,`TestId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='海龟交易系统趋势';

-- 正在导出表  tast.TurtleTradingSystemTrend 的数据：~0 rows (大约)
/*!40000 ALTER TABLE `TurtleTradingSystemTrend` DISABLE KEYS */;
/*!40000 ALTER TABLE `TurtleTradingSystemTrend` ENABLE KEYS */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
