﻿// Copyright 2016-2016 Amazon.com, Inc. or its affiliates. All Rights Reserved.
// Licensed under the Apache License, Version 2.0  (the "License"). You may not use this file except in compliance
// with the License. A copy of the License is located at http://aws.amazon.com/apache2.0/ or in the "license" file
// accompanying this file. This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied. See the License for the specific language governing permissions and limitations
// under the License.
using System.IO;
using ZetaLongPaths;

namespace AWSDeploymentAssistant
{
    public static class FileSystemUtil
    {
        internal static void CopyDirectory(ZlpDirectoryInfo source, ZlpDirectoryInfo destination, bool copySubDirs)
        {
            Assert.DirectoryExists(source);

            if (destination.Exists == false)
            {
                destination.Create();
            }

            ZlpFileInfo[] sourceFiles = source.GetFiles();

            foreach (ZlpFileInfo sourceFile in sourceFiles)
            {
                string destinationFilePath = Path.Combine(destination.FullName, sourceFile.Name);

                sourceFile.CopyTo(destinationFilePath, false);
            }

            if (copySubDirs)
            {
                ZlpDirectoryInfo[] sourceDirectories = source.GetDirectories();

                foreach (ZlpDirectoryInfo sourceDirectory in sourceDirectories)
                {
                    string destinationDirectoryPath = Path.Combine(destination.FullName, sourceDirectory.Name);

                    ZlpDirectoryInfo destinationDirectory = new ZlpDirectoryInfo(destinationDirectoryPath);

                    FileSystemUtil.CopyDirectory(sourceDirectory, destinationDirectory, copySubDirs);
                }
            }
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static void WriteFile(string path, byte[] contents, params string[] append)
        {
            using (var s = File.Create(path))
            {
                s.Write(contents, 0, contents.Length);
                s.Flush();
                s.Close();
            }

            if (append != null)
            {
                using (var s = File.OpenWrite(path))
                {
                    using (StreamWriter writer = new StreamWriter(s))
                    {
                        foreach (string value in append)
                        {
                            writer.WriteLine(value);
                        }
                    }
                }
            }
        }
    }
}