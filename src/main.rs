use argh::FromArgs;
use std::fs::File;
use std::io::{Read, Write, BufReader, BufRead};
use std::net::{TcpStream, TcpListener};
use std::thread;

#[derive(FromArgs)]
#[argh(description = "omp arguments")]
struct OmpArgs {
    #[argh(
        option,
        description = "the proxy mode to use. can be \"mantle\", \"optimantle\", or \"mantlefine\". leave blank to default to \"mantle\".",
        short = 'm'
    )]
    mode: Option<String>,

    #[argh(
        switch,
        description = "provides help with proxy modes (see \"--mode\"). prints a simple message that describes proxy mode functionality."
    )]
    mode_help: bool,

    #[argh(
        switch,
        description = "cleans up the hosts file (removes any rerouting from s.optifine.net)",
        short = 'c'
    )]
    clean: bool,
}

fn main() {
    println!("Reading launch arguments...");

    let args: OmpArgs = argh::from_env();

    if args.mode_help {
        println!(
            "\n\nProxy modes:
 - mantle: Default proxy configuration, displays only only Mantle capes.
 - optimantle: Displays Mantle capes if an OptiFine cape could not be resolved.
 - mantlefine: Displays OptiFine capes if a Mantle cape could not be resolved.\n\n"
        );

        return;
    }

    if args.clean {
        println!("\n\nCleaning up the hosts file...\n\n");
        let clean_hosts_path = resolve_hosts_path();
        clean_hosts(&clean_hosts_path, None);
        return;
    }

    let proxy_mode: String;

    if args.mode.is_none() {
        println!("Could not resolve argument: \"--mode\" (\"-m\")");
        println!("Setting mode to default: \"mantle\"");
        proxy_mode = "mantle".to_string();
    } else {
        proxy_mode = args.mode.unwrap();
    }

    println!(
        "
      .+=:.   .:=+.      
      +*****+*****+      
     -***=+***+=***-     
    .***=       =***.    
    +***         ***+    
   -***:         :***-   OpenMantleProxy v1.0.0
  .***=           =***.  by the Uranometrical Team
  +**+  .       .  ***+  
 =***.:+*=.   .=*+::***- 
.***++*****=:=*****++***.
:+*****=:+*****+:=*****+:
  :++=.   :=+=:   .=++:  
    "
    );
    println!("\nUsing proxy mode: {}", proxy_mode);

    let mut port: u16 = 3764;

    while !port_scanner::local_port_available(port) {
        port += 1;
    }

    println!("Resolved open port: {}", port);

    let listener = TcpListener::bind(format!("127.0.0.1:{}", port)).unwrap();

    println!("Binded to: 127.0.0.1: {}\n", port);

    let hosts_path = resolve_hosts_path();
    clean_hosts(
        &hosts_path,
        Some(format!("127.0.0.1:{} s.optifine.net", port).to_owned()),
    );

    for stream in listener.incoming() {
        match stream {
            Ok(stream) => {
                thread::spawn(|| {
                    handle_stream(stream);
                });
            }

            Err(e) => {
                println!("Error: {}", e);
            }
        }
    }
}

fn resolve_hosts_path() -> String {
    let mut hosts_path: String = String::new();

    if cfg!(windows) {
        // TODO: This is terrinly hard-coded. Fix... eventually?
        hosts_path = r"C:\Windows\System32\Drivers\etc\hosts".to_owned();

        println!("Presumably running on Windows.");
    } else if cfg!(unix) {
        hosts_path = "/etc/hosts".to_owned();

        println!("Presumably running on *Nix (MacOS, Linux, etc.)");
    }

    println!("Current hosts path: {}", hosts_path);

    while !std::path::Path::new(&hosts_path).exists() {
        println!("Hosts file path could not be resolved, please input the path manually:");
        hosts_path = text_io::read!("{}\n");
    }

    hosts_path
}

fn clean_hosts(hosts_path: &String, additional_line: Option<String>) {
    let input =
        File::open(hosts_path).expect("Could not open hosts, is the process lacking permission?");
    let buffered = BufReader::new(input);
    let mut lines = std::vec::Vec::new();

    for line in buffered.lines() {
        let s = line.expect("Couldn't get line as string, what?");

        if !s.ends_with("s.optifine.net") {
            lines.push(s);
        }
    }

    if additional_line.is_some() {
        lines.push(additional_line.unwrap());
    }

    write!(
        File::create(hosts_path).expect("Could not open hosts, is the process lacking permission?"),
        "{}",
        format!("{}", lines.join("\n"))
    ).unwrap();
}

fn handle_stream(stream: TcpStream) {
    stream_read(&stream);
    stream_write(stream);
}

fn stream_read(mut stream: &TcpStream) {
    let mut buf = [0u8 ;4096];
    match stream.read(&mut buf) {
        Ok(_) => {
            let req_str = String::from_utf8_lossy(&buf);
            println!("{}", req_str);
            },
        Err(e) => println!("Unable to read stream: {}", e),
    }
}

fn stream_write(mut stream: TcpStream) {
    let response = b"HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n<html><body>Hello world</body></html>\r\n";
    match stream.write(response) {
        Ok(_) => println!("Response sent"),
        Err(e) => println!("Failed sending response: {}", e),
    }
}

/*async fn shutdown_signal() {
    tokio::signal::ctrl_c()
    .await
    .expect("Failed to install CTRL+C signal handler");
}*/
