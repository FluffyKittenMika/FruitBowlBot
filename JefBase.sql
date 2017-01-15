-- phpMyAdmin SQL Dump
-- version 4.6.3
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: Jan 15, 2017 at 06:20 AM
-- Server version: 5.5.51-MariaDB
-- PHP Version: 5.5.38

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `JefBase`
--

-- --------------------------------------------------------

--
-- Table structure for table `Chat`
--

CREATE TABLE `Chat` (
  `CHAT` text,
  `ID` bigint(20) UNSIGNED NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `Quotes`
--

CREATE TABLE `Quotes` (
  `ID` bigint(20) UNSIGNED NOT NULL COMMENT 'Index',
  `QUOTE` text COMMENT 'The Quote',
  `SUBMITTER` varchar(120) DEFAULT NULL,
  `CHANNEL` varchar(255) DEFAULT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `Vods`
--

CREATE TABLE `Vods` (
  `ID` bigint(20) UNSIGNED NOT NULL COMMENT 'Index',
  `NAME` varchar(400) DEFAULT NULL COMMENT 'Vod Name',
  `PATH` varchar(2000) DEFAULT NULL COMMENT 'path',
  `UPDATED` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Update time',
  `TAGS` text COMMENT 'TAGS'
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Chat`
--
ALTER TABLE `Chat`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `Quotes`
--
ALTER TABLE `Quotes`
  ADD PRIMARY KEY (`ID`);
ALTER TABLE `Quotes` ADD FULLTEXT KEY `QUOTE` (`QUOTE`);

--
-- Indexes for table `Vods`
--
ALTER TABLE `Vods`
  ADD PRIMARY KEY (`ID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `Chat`
--
ALTER TABLE `Chat`
  MODIFY `ID` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=88938;
--
-- AUTO_INCREMENT for table `Quotes`
--
ALTER TABLE `Quotes`
  MODIFY `ID` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Index', AUTO_INCREMENT=1771;
--
-- AUTO_INCREMENT for table `Vods`
--
ALTER TABLE `Vods`
  MODIFY `ID` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Index';
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
