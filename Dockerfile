FROM eosio/eos-dev

WORKDIR /workdir

# copy files
RUN mkdir /home/dasdaq_eos
RUN mkdir /home/dasdaq_eos/agent
RUN mkdir /home/dasdaq_eos/instances
RUN mkdir /home/dasdaq_eos/temp
RUN mkdir /home/dasdaq_eos/contracts

COPY ./Dasdaq.Dev.Agent /home/dasdaq_eos/agent
COPY ./Dasdaq.Dev.InvokeContractSample /home/dasdaq_eos/instances/sample

# Install .NET Core
RUN wget -P /home -O libicu55.deb http://archive.ubuntu.com/ubuntu/pool/main/i/icu/libicu55_55.1-7ubuntu0.4_amd64.deb
RUN dpkg -i /home/libicu55.deb
RUN wget -P /home -O packages-microsoft-prod.deb -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-get update
RUN apt-get install dotnet-sdk-2.1
