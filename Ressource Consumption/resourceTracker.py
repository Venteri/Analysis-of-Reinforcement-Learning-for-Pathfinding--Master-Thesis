import psutil
import time
import subprocess
import csv
import os

# Define the path to the program you want to run
program_path = "path/to/your/program.exe"

# Set the time interval for writing to the CSV file
time_interval = 10  # seconds

# Create a folder to store the CSV files if it doesn't exist
folder_name = "Ressource Tracking"
if not os.path.exists(folder_name):
    os.makedirs(folder_name)

# Get the current time to use as part of the CSV file name
current_time = time.strftime("%Y%m%d-%H%M%S")

# Create the CSV file and write the header row
csv_file_name = os.path.join(folder_name, f"resource_consumption_{current_time}.csv")
with open(csv_file_name, mode='w', newline='') as file:
    writer = csv.writer(file)
    writer.writerow(['CPU usage', 'GPU usage', 'RAM usage', 'Watt usage'])

# Start tracking the resource consumption
cpu_percentages = []
gpu_percentages = []
ram_usage = []
watt_usage = []

# Run the program
p = subprocess.Popen(program_path)

# Track the resource consumption and write to the CSV file every time_interval seconds
while p.poll() is None:
    # CPU usage
    cpu_percentages.append(psutil.cpu_percent())

    # GPU usage (assuming NVIDIA GPU)
    gpu_percentages.append(psutil.sensors.nvidia_smi().gpu_util)

    # RAM usage
    ram_usage.append(psutil.virtual_memory().percent)

    # Watt usage (assuming Windows OS)
    watt_usage.append(psutil.sensors.power_supply().power_plugged)

    # Write to the CSV file every time_interval seconds
    if len(cpu_percentages) % (time_interval * 10) == 0:
        with open(csv_file_name, mode='a', newline='') as file:
            writer = csv.writer(file)
            writer.writerow([cpu_percentages[-1], gpu_percentages[-1], ram_usage[-1], watt_usage[-1]])

    # Wait for a short time before checking the resource consumption again
    time.sleep(0.1)

# Write the final resource consumption data to the CSV file
with open(csv_file_name, mode='a', newline='') as file:
    writer = csv.writer(file)
    for i in range(len(cpu_percentages)):
        writer.writerow([cpu_percentages[i], gpu_percentages[i], ram_usage[i], watt_usage[i]])
