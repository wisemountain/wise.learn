Dijkstra. Passed away. 

# The role of programming languages

rules of the game. Euclid's algorithm. 

Algorithm is the rule of the game. System is not an algorithm, though. 
That's why discipline is essential. 

Formal notation technique --> language. 
mathematical object. what kind of mathematical object is a programming language? 

It is a sequence of transformation of states. Imperative and Hoare logic perspective. 


the intellectual manageability of the programs expressed. 
Is this problem solved yet? Nope. 

We need an elegant language for each domain, for each algorithm, for each ADT using existing language constructs. 

Efficient algorithm, elegant interface, correct (verified) system. 

Procedure! was the construct that can make interface elegant and correct system. 
Correctness can be enhanced through intellectual manageability of the system. 

Dijkstra is a strong advocate of elegance through math and structure. 



# States and their characterization

state space. 

```c++
	static segment_buffer			seg_buffer_accessor_;

	tcp_protocol*					protocol_ = nullptr;
	tcp::socket						socket_;
	std::string						local_addr_;
	std::string						remote_addr_;
	std::string						desc_;

	mutable lock_type				session_mutex_;
	bool							recving_ = false; 
	bool							sending_ = false;
	std::array<uint8_t, 32 * 1024>	recv_buf_;			

	lock_type						send_mutex_;
	segment_buffer					send_buffer_;
	std::size_t						send_request_size_ = 0;
	std::vector<seg*>				sending_segs_;
	std::vector<boost::asio::const_buffer> sending_bufs_;

	error_code						last_error_;
	bool							destroyed_ = false;
```

what are state space of the above object? 
what are dynamic ones? 
what are automata partitions? 

mostly static: 
    - protocol_, socket_, local_addr_, remote_addr_, desc_, session_mutex_
    - created -> destroyed automata

recving state:
    - recving_, recv_buf_, 
    - recv automata

sending state: 
    - send_buffer_, send_segs_, sending_bufs_
    - send automata

error: 
    - last_error_ 
    - error automata connected with recv automata and send automata

오브젝트 상태와 오토마타를 나누고, 다시 연결하는 부분을 찾아서 HFSM 또는 statechart로 그린다. 현재 상태기계의 상태에 따른 invariance로 증명한다. 



