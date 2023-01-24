#!/bin/bash

solution="eShopOnContainers-ServicesAndWebApps.sln"
outfile="DockerfileSolutionRestore1.txt"

echo "COPY \"$solution\" \"$solution\"" >> $outfile 
echo "" >> $outfile

find . -name "*.csproj" | cut -c3- | while read fname; do
  echo "COPY \"$fname\" \"$fname\"" >> $outfile 
done

echo "" >> $outfile

find . -name "*.dcproj" | cut -c3- | while read fname; do
  echo "COPY \"$fname\" \"$fname\"" >> $outfile 
done

echo "" >> $outfile
echo "COPY \"NuGet.config\" \"NuGet.config\"" >> $outfile

echo "" >> $outfile
echo "RUN dotnet restore \"$solution\"" >> $outfile