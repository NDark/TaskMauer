-- phpMyAdmin SQL Dump
-- version 4.2.12deb2
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Dec 07, 2018 at 02:00 PM
-- Server version: 5.6.25-0ubuntu0.15.04.1
-- PHP Version: 5.6.4-4ubuntu6.3

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `taskmauerserver`
--

-- --------------------------------------------------------

--
-- Table structure for table `tb_TaskBundles`
--

CREATE TABLE IF NOT EXISTS `tb_TaskBundles` (
`TaskIndex` int(11) NOT NULL,
  `Title` text COLLATE utf8_unicode_ci NOT NULL,
  `ProjectKey` varchar(40) COLLATE utf8_unicode_ci NOT NULL,
  `Assignee` text COLLATE utf8_unicode_ci NOT NULL,
  `TimeStamp` date NOT NULL,
  `ProgressInt` int(11) NOT NULL,
  `ProgressFloat` float NOT NULL,
  `Link` text COLLATE utf8_unicode_ci NOT NULL,
  `PositionStr` text COLLATE utf8_unicode_ci NOT NULL,
  `IsPin` tinyint(1) NOT NULL,
  `ParentID` int(11) NOT NULL,
  `RelativesStr` text COLLATE utf8_unicode_ci NOT NULL,
  `NeedFollowID` int(11) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `tb_TaskBundles`
--

INSERT INTO `tb_TaskBundles` (`TaskIndex`, `Title`, `ProjectKey`, `Assignee`, `TimeStamp`, `ProgressInt`, `ProgressFloat`, `Link`, `PositionStr`, `IsPin`, `ParentID`, `RelativesStr`, `NeedFollowID`) VALUES
(1, 'abcd', 'TestProject', 'b', '0000-00-00', 0, 0, 'c', '-5.377298,-1.098416,0', 1, 0, '"\\"\\\\\\"\\\\\\\\\\\\\\"\\\\\\\\\\\\\\"\\\\\\"\\""', 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `tb_TaskBundles`
--
ALTER TABLE `tb_TaskBundles`
 ADD PRIMARY KEY (`TaskIndex`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `tb_TaskBundles`
--
ALTER TABLE `tb_TaskBundles`
MODIFY `TaskIndex` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=2;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
